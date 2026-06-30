using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileSystemGlobbing;
using System.Diagnostics;
using System.Text;

namespace ContractorsDesk.Services
{
	public class ImportDataService : BaseService, IImportDataService
	{
		private readonly ClientDbContext testClientDbContext;
		private readonly ClientDbContext liveClientDbContext;
		private readonly INotificationService notificationService;
		private List<SyncEntityBase> SyncEntities = new List<SyncEntityBase>();
		private List<string> RecordsDone = new List<string>();
		public ImportDataService(
			INotificationService notificationService,
			IConfiguration configuration,
			ClientDbContext clientDbContext) : base(mapper: null, clientDataDbContext: clientDbContext, configuration: configuration, notificationService: notificationService)
		{
			var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
			optionsBuilder.UseSqlServer(configuration.GetConnectionString("default"));

			var liveOptionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
			liveOptionsBuilder.UseSqlServer(configuration.GetConnectionString("live"));

			testClientDbContext = new ClientDbContext(optionsBuilder.Options);
			liveClientDbContext = new ClientDbContext(liveOptionsBuilder.Options);
			this.notificationService = notificationService;

		}

		#region Public

		[AutomaticRetry(Attempts = 0)]
		public async Task ImportDataFromProductionDatabase(SignalRMessageModel? signalRMessageModel = null)
		{
			this.signalRMessageModel = signalRMessageModel;

			await PopulateSyncEntities();

			if (await ResetDatabaseTables())
			{
				var startMessage = $"🔄 IMPORTING DATA FROM PRODUCTION";

				await SendNotification(startMessage);
				await InvokeImportAsync();
				await UpdateNotification(startMessage, $"✔️ IMPORTING DATA FROM PRODUCTION");
				await SendNotification($"\n✔️ *Import completed successfully!*");
			}
		}
		#endregion

		#region Private
		private async Task<bool> ResetDatabaseTables()
		{
			try
			{
				var message = "REMOVING CURRENT DATA";
				await SendNotification($"🔄 {message}");

				for (var lastIndex = this.SyncEntities.Count() - 1; lastIndex > -1; lastIndex--)
				{
					var entity = this.SyncEntities[lastIndex];
					var tableName = entity.TableName;
					var action = entity.ResetCommand == "Delete" ? "DELETE FROM" : "TRUNCATE TABLE";

					var sqlSb = new StringBuilder();
					sqlSb.AppendLine($"{action} {entity.TableName};");

					if (entity.IdentityInsert)
						sqlSb.AppendLine($"DBCC CHECKIDENT ('{tableName}', RESEED, 0);");

					var query = sqlSb.ToString();

					var deleteMessage = $"Deleting records from {tableName}";

					var inprogressMessage = $"    🔄 {deleteMessage}";
					var completedMessage = $"   ✔️ {deleteMessage}";

					await SendNotification(inprogressMessage);
					await testClientDbContext.Database.ExecuteSqlRawAsync(query);
					await UpdateNotification(completedMessage);

				}

				await UpdateNotification($"🔄 {message}", $"✔️ {message}");
				return true;
			}
			catch (Exception ex)
			{
				await RaiseErrorNotification(ex.Message);
				return false;
			}
		}
		private async Task RunSyncDataAsync<T>(string tableName, DbSet<T> liveDbSet, DbSet<T> testDbSet) where T : class
		{
			try
			{
				var currentEntity = SyncEntities.FirstOrDefault(e => e.TableName == tableName);
				if(currentEntity != null)
				{
					//refactor here and use data paging for liveDbSet.
					// pass the liveDbSet to InsertWithIdentityAsync and ProcessImportData methods
					// perform the data paging in those methods

					var nextEntity = SyncEntities.FirstOrDefault(e => SyncEntities.IndexOf(e) == SyncEntities.IndexOf(currentEntity) + 1);
					var totalCount = await liveDbSet.CountAsync();

					var inprogressMessage = $"    🔄 Importing {tableName} (0 out of {totalCount} records)";
					var completedMessage = $"    ✔️ Importing {tableName} ({totalCount} out of {totalCount} records)";

					await SendNotification(inprogressMessage);

					
					if (currentEntity.IdentityInsert)
					{
						await InsertWithIdentityAsync(testClientDbContext, tableName, liveDbSet);
					}
					else
					{
						await ProcessImportData(tableName, liveDbSet, testClientDbContext, inprogressMessage);
					}

					
					await UpdateNotification(completedMessage);

					SyncEntities.Remove(currentEntity!);

					if (nextEntity != null) 
						await InvokeImportAsync(nextEntity);
					
				}

			}
			catch (Exception ex)
			{
				await RaiseErrorNotification(ex.Message);
				throw;
			}
		}
		private async Task<bool> ProcessImportData<T>(string tableName, DbSet<T> liveDbSet, DbContext dbContext, string inprogressMessage) where T : class
		{
			try
			{
				var totalCount = await liveDbSet.CountAsync();
				var recordsAffected = 0;

				while(recordsAffected < totalCount)
				{
					var currentBatch = await liveDbSet
						.AsNoTracking()
						.Skip(recordsAffected)
						.Take(10000)
						.ToListAsync();

					await dbContext.AddRangeAsync(currentBatch);
					await testClientDbContext.SaveChangesAsync();

					recordsAffected += currentBatch.Count;
					var percentageComplete = (recordsAffected) * 100 / totalCount;

					var message = $"    🔄 Importing {tableName} ({recordsAffected} out of {totalCount} records)";
					await UpdateNotification(message);
				}

				return true;
			}
			catch(Exception ex)
			{
				await RaiseErrorNotification($"{ex.InnerException.Message} : {ex.InnerException.StackTrace}");
				return false;
			}
		}
		private async Task<bool> InsertWithIdentityAsync<T>(DbContext dbContext, string tableName, DbSet<T> liveDbSet) where T : class
		{
			using (var transaction = await dbContext.Database.BeginTransactionAsync())
			{
				try
				{
					var entities = await liveDbSet.ToListAsync();
					// Enable IDENTITY_INSERT
					await dbContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [dbo].{tableName} ON;");

					// Insert data into the table
					await dbContext.Set<T>().AddRangeAsync(entities);
					await dbContext.SaveChangesAsync();

					// Disable IDENTITY_INSERT
					await dbContext.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [dbo].{tableName} OFF;");

					// Commit the transaction
					await transaction.CommitAsync();
					return true;
				}
				catch (Exception ex)
				{
					await transaction.RollbackAsync();
					await RaiseErrorNotification(ex.Message);
					return false;
				}
			}
		}	
		private async Task PopulateSyncEntities()
		{
			await Task.Run(() =>
			{
				this.SyncEntities = new List<SyncEntityBase>
				{
					new SyncEntity<Qbclass>
					{
						TableName = "QbClasses",
						LiveDbSet = liveClientDbContext.Qbclasses,
						TestDbSet = testClientDbContext.Qbclasses
					},
					new SyncEntity<Qbcustomer>
					{
						TableName = "QBCustomers",
						LiveDbSet = liveClientDbContext.Qbcustomers,
						TestDbSet = testClientDbContext.Qbcustomers
					},
					new SyncEntity<SubContractor>
					{
						TableName = "SubContractors",
						LiveDbSet = liveClientDbContext.SubContractors,
						TestDbSet = testClientDbContext.SubContractors
					},
					new SyncEntity<ClientDocument>
					{
						TableName = "ClientDocuments",
						LiveDbSet = liveClientDbContext.ClientDocuments,
						TestDbSet = testClientDbContext.ClientDocuments
					},
					new SyncEntity<Email>
					{
						TableName = "Emails",
						LiveDbSet = liveClientDbContext.Emails,
						TestDbSet = testClientDbContext.Emails
					},
					new SyncEntity<EmailAttachment>
					{
						TableName = "EmailAttachments",
						IdentityInsert = true,
						LiveDbSet = liveClientDbContext.EmailAttachments,
						TestDbSet = testClientDbContext.EmailAttachments
					},
					new SyncEntity<ProposalTemplate>
					{
						TableName = "ProposalTemplates",
						LiveDbSet = liveClientDbContext.ProposalTemplates,
						TestDbSet = testClientDbContext.ProposalTemplates
					},
					new SyncEntity<ProposalTemplateUserDefault>
					{
						TableName = "ProposalTemplateUserDefault",
						LiveDbSet = liveClientDbContext.ProposalTemplateUserDefaults,
						TestDbSet = testClientDbContext.ProposalTemplateUserDefaults
					},
					new SyncEntity<ProposalTemplatesLineItem>
					{
						TableName = "ProposalTemplatesLineItems",
						LiveDbSet = liveClientDbContext.ProposalTemplatesLineItems,
						TestDbSet = testClientDbContext.ProposalTemplatesLineItems
					},
					new SyncEntity<Proposal>
					{
						TableName = "Proposals",
						LiveDbSet = liveClientDbContext.Proposals,
						TestDbSet = testClientDbContext.Proposals
					},
					new SyncEntity<ProposalLine>
					{
						TableName = "ProposalLines",
						LiveDbSet = liveClientDbContext.ProposalLines,
						TestDbSet = testClientDbContext.ProposalLines
					},
					new SyncEntity<ProposalLinesHistory>
					{
						TableName = "ProposalLinesHistory",
						LiveDbSet = liveClientDbContext.ProposalLinesHistories,
						TestDbSet = testClientDbContext.ProposalLinesHistories
					},
					new SyncEntity<Invoice>
					{
						TableName = "Invoices",
						LiveDbSet = liveClientDbContext.Invoices,
						TestDbSet = testClientDbContext.Invoices
					},
					new SyncEntity<InvoiceItem>
					{
						TableName = "InvoiceItems",
						LiveDbSet = liveClientDbContext.InvoiceItems,
						TestDbSet = testClientDbContext.InvoiceItems
					},
					new SyncEntity<JobBalance>
					{
						TableName = "JobBalances",
						LiveDbSet = liveClientDbContext.JobBalances,
						TestDbSet = testClientDbContext.JobBalances
					},
					new SyncEntity<ProjectTotal>
					{
						TableName = "ProjectTotals",
						LiveDbSet = liveClientDbContext.ProjectTotals,
						TestDbSet = testClientDbContext.ProjectTotals
					},
					new SyncEntity<ProjectDocument>
					{
						TableName = "ProjectDocuments",
						LiveDbSet = liveClientDbContext.ProjectDocuments,
						TestDbSet = testClientDbContext.ProjectDocuments
					},
					new SyncEntity<ProjectSupervisor>
					{
						TableName = "ProjectSupervisors",
						IdentityInsert = true,
						LiveDbSet = liveClientDbContext.ProjectSupervisors,
						TestDbSet = testClientDbContext.ProjectSupervisors
					},
					new SyncEntity<ActionItem>
					{
						TableName = "ActionItems",
						IdentityInsert = true,
						LiveDbSet = liveClientDbContext.ActionItems,
						TestDbSet = testClientDbContext.ActionItems
					},
					new SyncEntity<ChangeOrder>
					{
						TableName = "ChangeOrders",
						LiveDbSet = liveClientDbContext.ChangeOrders,
						TestDbSet = testClientDbContext.ChangeOrders
					},
					new SyncEntity<ActionItemsSupervisor>
					{
						TableName = "ActionItemsSupervisors",
						LiveDbSet = liveClientDbContext.ActionItemsSupervisors,
						TestDbSet = testClientDbContext.ActionItemsSupervisors
					},
					new SyncEntity<ActionItemCostChange>
					{
						TableName = "ActionItemCostChange",
						IdentityInsert = true,
						LiveDbSet = liveClientDbContext.ActionItemCostChanges,
						TestDbSet = testClientDbContext.ActionItemCostChanges
					},
					new SyncEntity<ActionItemScheduleChange>
					{
						TableName = "ActionItemScheduleChange",
						IdentityInsert = true,
						LiveDbSet = liveClientDbContext.ActionItemScheduleChanges,
						TestDbSet = testClientDbContext.ActionItemScheduleChanges
					},
					new SyncEntity<ProjectSchedule>
					{
						TableName = "ProjectSchedules",
						LiveDbSet = liveClientDbContext.ProjectSchedules,
						TestDbSet = testClientDbContext.ProjectSchedules
					},
					new SyncEntity<ProjectScheduleTask>
					{
						TableName = "ProjectScheduleTasks",
						LiveDbSet = liveClientDbContext.ProjectScheduleTasks,
						TestDbSet = testClientDbContext.ProjectScheduleTasks
					},
					new SyncEntity<ProjectScheduleDelay>
					{
						TableName = "ProjectScheduleDelays",
						LiveDbSet = liveClientDbContext.ProjectScheduleDelays,
						TestDbSet = testClientDbContext.ProjectScheduleDelays
					},
					new SyncEntity<ProjectJournal>
					{
						TableName = "ProjectJournal",
						LiveDbSet = liveClientDbContext.ProjectJournals,
						TestDbSet = testClientDbContext.ProjectJournals
					},
					new SyncEntity<Qbaccount>
					{
						TableName = "QbAccounts",
						LiveDbSet = liveClientDbContext.Qbaccounts,
						TestDbSet = testClientDbContext.Qbaccounts
					},
					new SyncEntity<Qbtransaction>
					{
						TableName = "QbTransactions",
						ResetCommand = "Truncate",
						LiveDbSet = liveClientDbContext.Qbtransactions,
						TestDbSet = testClientDbContext.Qbtransactions
					},
				};
			});
		}
		private async Task InvokeImportAsync(SyncEntityBase? syncEntity = null)
		{
			if (syncEntity == null)
			{
				syncEntity = SyncEntities.FirstOrDefault();
			}

			var tableName = syncEntity.TableName;
			var liveDbSet = syncEntity.GetLiveDbSet();
			var testDbSet = syncEntity.GetTestDbSet();

			if (liveDbSet != null && testDbSet != null)
			{
				// Get the type of the entity (T)
				var entityType = liveDbSet.GetType().GetGenericArguments()[0];

				// Dynamically invoke the generic RunSyncDataAsync method
				var method = typeof(ImportDataService).GetMethod(nameof(RunSyncDataAsync), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				var genericMethod = method.MakeGenericMethod(entityType);

				await (Task)genericMethod.Invoke(this, new object[] { tableName, liveDbSet, testDbSet });

			}
		}
		private abstract class SyncEntityBase
		{
			public string TableName { get; set; }
			public bool IdentityInsert { get; set; }
			public string ResetCommand { get; set; } = "Delete";
			public abstract object GetLiveDbSet();
			public abstract object GetTestDbSet();
		}
		private class SyncEntity<T> : SyncEntityBase where T : class
		{
			public DbSet<T>? LiveDbSet { get; set; }
			public DbSet<T>? TestDbSet { get; set; }

			public override object GetLiveDbSet() => LiveDbSet!;
			public override object GetTestDbSet() => TestDbSet!;
		}
		#endregion
	}
}
