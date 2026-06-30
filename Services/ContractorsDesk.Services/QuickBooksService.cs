using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using EFCore.BulkExtensions;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pipelines.Sockets.Unofficial.Arenas;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Http.Headers;
using System.Text;
using ContractorsDesk.Core.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class QuickBooksService : BaseService, IQuickBooksService
	{
		private IEnumerable<Qbaccount> dbAccounts;
		private IEnumerable<Qbvendor> dbVendors;
		private IEnumerable<Qbclass> dbClasses;
		private IEnumerable<Qbcustomer> dbCustomers;
		private INotificationService notificationService;
		private QuickBooksToken? QuickBooksToken = null;
		private List<string> RecordsDone = new List<string>();
		private bool deleteQbTransactions = false;

		private List<string> QuickbooksEntityDoneItems { get; set; } = new List<string>();
		private List<string> ProjectProposalDoneItems { get; set; } = new List<string>();
		private List<string> ProjectTotalsDoneItems { get; set; } = new List<string>();
		private List<Qbclass> ActiveJobs { get; set; } = new List<Qbclass>();

		public QuickBooksService(
			INotificationService notificationService,
			IConfiguration configuration,
			MasterDbContext masterDbContext,
			ClientDbContext clientDbContext) : base(
				mapper: null, 
				clientDataDbContext: clientDbContext, 
				masterDbContext: masterDbContext,
				configuration: configuration, 
				notificationService: notificationService)
		{
			this.configuration = configuration;
			this.dbAccounts = new List<Qbaccount>();
			this.dbVendors = new List<Qbvendor>();
			this.dbClasses = new List<Qbclass>();
			this.dbCustomers = new List<Qbcustomer>();
			this.notificationService = notificationService;
			this.ClientDbContext = clientDbContext;
		}

		/// <summary>
		/// Run data sync
		/// </summary>
		[AutomaticRetry(Attempts = 0)]
		public async Task RunDataSync(SignalRMessageModel? signalRMessageModel)
		{
			try
			{
				this.signalRMessageModel = signalRMessageModel;
				//START LOOP HERE
				var companyQuickbooksSettings = await this.masterDbContext.QuickbooksSettings
					.AsNoTracking()
					.ToListAsync();

				foreach (var setting in companyQuickbooksSettings)
				{
					var connectionString = await masterDbContext.ConnectionStrings
						.AsNoTracking()
						.FirstOrDefaultAsync(c => c.CompanyId == setting.CompanyId);
					if (connectionString == null) continue;

					var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
					optionsBuilder.UseSqlServer(connectionString.Value);

					this.ClientDbContext = new ClientDbContext(optionsBuilder.Options);


					this.QuickBooksToken = await ClientDbContext.QuickBooksTokens.FirstOrDefaultAsync();
					if (this.QuickBooksToken == null) throw new Exception("QuickBooks Account is not yet connected.");

					var qbDataSyncSetting = await ClientDbContext.SysDataSyncSettings.FirstOrDefaultAsync(s => s.DataSyncName == "Quickbooks Sync");

					this.notificationService.ClientDbContext = this.ClientDbContext;
					if (this.signalRMessageModel != null && this.signalRMessageModel.NotifyOnStart)
					{
						
						this.signalRMessageModel.NotificationId = await this.notificationService.SendDataSyncStartedNotification(this.signalRMessageModel);
					}


					await SyncQuickBooksAsync(qbDataSyncSetting);

					if (qbDataSyncSetting.IsFirstRun == true)
					{
						await SyncActiveJobs(signalRMessageModel);
					}

					await SyncProposals(signalRMessageModel);
					await SyncProjectTotals(signalRMessageModel);
					await SendCompletedDataSyncStatusNotification();
				}
				// end loop here
			}
			catch (Exception ex)
			{
				await RaiseErrorNotification(ex.Message);
			}
		}
		[AutomaticRetry(Attempts = 0)]
		public async Task SyncProposals(SignalRMessageModel? signalRMessageModel = null)
		{
			this.signalRMessageModel = signalRMessageModel;
			await SendProposalSyncStatusNotification();

			var activeQuickbooksJobs = ClientDbContext.Qbclasses
				.Where(c => (c.ActiveJobs == true || c.ActiveSpecJobs == true) 
							&& c.IsDeleted != true 
							&& c.IsArchived != true 
							&& c.ListId != "NonQBOProject")
				.AsNoTracking().ToList();

			var jobsWithoutProposal = await ClientDbContext
				.Qbclasses
				.AsNoTracking()
				.Where(c => activeQuickbooksJobs.Select(aj => aj.Id).Contains(c.Id) && !ClientDbContext.Proposals.Any(p => p.QbclassId == c.Id))
				.ToListAsync();

			foreach (var job in jobsWithoutProposal)
			{
				var projectName = job.Name;
				var projectId = job.Id;
				try
				{
					await SendProposalSyncStatusNotification(projectName: projectName, processing: true);
					await this.ClientDbContext.Database.ExecuteSqlRawAsync("EXEC spCreateProposalFromJob @QbClassId = {0}", projectId);
					await SendProposalSyncStatusNotification(projectName: projectName, done: true);
				}
				catch (Exception) { throw; }
			}

			if (jobsWithoutProposal.Count() > 0)
			{
				var userIds = await ClientDbContext.Users.Where(u => u.RoleId == (int)Roles.CompanyOwner).Select(u => u.Id).ToListAsync();
				var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
				var relatedUrl = "/"; //await GetActionItemUrl(actionItemDto.Id);

				string notificationMessage = $"<strong>System Administrator:</strong> The system has identified one or more QuickBooks classes that were downloaded but do not currently have existing proposal records in our database. Please review the list of Active Projects and Proposals from the dashboard.";

				var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, string.Empty, 1, relatedUrl: relatedUrl, userIds: userIds);

				await notificationService.AddNewNotification(notificationRequest);
			}

			await SendProposalSyncStatusNotification(completed:true);
		}
		[AutomaticRetry(Attempts = 0)]
		public async Task SyncProjectTotals(SignalRMessageModel? signalRMessageModel = null)
		{
			this.signalRMessageModel = signalRMessageModel;
			if (ActiveJobs.Count == 0)
			{
				ActiveJobs = ClientDbContext.Qbclasses.Where(c => (c.ActiveJobs == true || c.ActiveSpecJobs == true) && c.IsDeleted != true && c.IsArchived != true).AsNoTracking().ToList();
			}

			//this.CurrentTelegramMessage = new TelegramMessageModel();
			var message = string.Empty;
			ClientDbContext.JobBalances.RemoveRange(ClientDbContext.JobBalances);

			await SendProjectTotalsSyncStatusNotification();
			try
			{
				var classListIds = ActiveJobs.Select(c => c.ListId).ToList()
					.Select(s => long.TryParse(s, out var n) ? n : (long?)null)
					.Where(n => n.HasValue)
					.Select(n => n.Value)
					.ToList();

				var response = await this.GetProfitAndLossReportAsync(classListIds);

				var reportData = JsonConvert.DeserializeObject<ProfitAndLossReportModel>(response.ToString());

				var columns = reportData.Columns.Column;
				var filteredColumns = columns.Skip(1).Take(columns.Count() - 2).ToList();

				var netIncome = reportData.Rows.Row.Where(r => r.Group.ToUpper() == "NETINCOME").FirstOrDefault();
				var netIncomeColData = netIncome?.Summary.ColData;
				var netIncomeSummary = netIncomeColData.Skip(1).Take(netIncomeColData.Count() - 2).ToList();

				int counter = 0;
				foreach (var columnName in filteredColumns)
				{
					var activeProject = ActiveJobs.FirstOrDefault(ActiveJobs => ActiveJobs.ListId == columnName.MetaData[0].Value);
					if (activeProject != null)
					{
						await SendProjectTotalsSyncStatusNotification(projectName: activeProject.Name, processing:true);
						var projectTotal = decimal.Parse(netIncomeSummary[counter].Value?.ToString());

						var jobBalance = new JobBalance
						{
							Id = Guid.NewGuid(),
							Balance = projectTotal,
							JobId = activeProject.Id,
							DateUpdated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
						};

						ClientDbContext.JobBalances.Add(jobBalance);
						await ClientDbContext.SaveChangesAsync();
						Thread.Sleep(250);
						await SendProjectTotalsSyncStatusNotification(projectName: activeProject.Name, done: true);
					}

					counter++;
				}

				await this.ClientDbContext.Database.ExecuteSqlRawAsync("DELETE FROM ProjectThresholds");
				foreach (var activeJob in ActiveJobs)
                {
                    await this.ClientDbContext.Database.ExecuteSqlRawAsync("EXEC spSynchProjectThresholds @ProjectId = {0}", activeJob.Id);
                }

				await SendProjectTotalsSyncStatusNotification(completed: true);
			}
			catch (Exception) { throw; }

		}
		[AutomaticRetry(Attempts = 0)]
		public async Task SyncActiveJobs(SignalRMessageModel? signalRMessageModel = null)
		{
			try
			{
				this.signalRMessageModel = signalRMessageModel;
				var qbClasses = await ClientDbContext.Qbclasses.ToListAsync();
				var qbTransactions = await ClientDbContext.Qbtransactions
					.Where(t => t.TransactionDate >= TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().AddDays(-60))
					.AsNoTracking()
					.Select(t => new { t.ClassId })
					.ToListAsync();

				await SendActiveJobsStatusNotification();
				foreach (var qbClass in qbClasses)
				{
					var isActiveJob = qbTransactions.Any(t => t.ClassId == qbClass.Id);
					qbClass.ActiveJobs = isActiveJob;
					qbClass.IsActive = isActiveJob;

					if (isActiveJob)
					{
						await SendActiveJobsStatusNotification(processing: true, projectName: qbClass.Name);
						ActiveJobs.Add(qbClass);
						await ClientDbContext.SaveChangesAsync();
						await SendActiveJobsStatusNotification(done: true, projectName: qbClass.Name);
					}
				}

				await SendActiveJobsStatusNotification(completed:true);
			}
			catch (Exception ex) {
				var message = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
				message += ex.InnerException != null ? ex.InnerException.StackTrace : ex.StackTrace;
				await RaiseErrorNotification(message);
			}
		}
		public async Task<object> GetProfitAndLossReportAsync(List<long> qbClasslistIds)
		{
			var quickbooksToken = await this.ClientDbContext.QuickBooksTokens.FirstOrDefaultAsync();

			if (quickbooksToken == null) throw new Exception("QuickBooks not connected.");

			var token = await RefreshQuickBooksTokenAsync(quickbooksToken);

			var classIds = string.Join(",", qbClasslistIds);

			var baseUrl = configuration.GetSection("QuickBooks").GetValue<string>("BaseUrl")
					 ?? throw new Exception("QuickBooks BaseUrl is missing.");

			var url = $"{baseUrl}/v3/company/{token.RealmId}/reports/ProfitAndLoss?accounting_method=Cash&date_macro=All&summarize_column_by=Classes&class={classIds}";

			using var client = new RestClient(url);
			var request = new RestRequest { Method = Method.Get };

			request.AddHeader("Authorization", $"Bearer {token.AccessToken}");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/text");

			var response = await client.ExecuteAsync(request);
			if (!response.IsSuccessful)
			{
				throw new Exception($"QuickBooks API request failed: {response.StatusCode} - {response.Content}");
			}

			return response.Content ?? "{}";

		}
		public async Task<bool> HasQuickBooksAccountConnected()
		{
			var token = await this.ClientDbContext.QuickBooksTokens.FirstOrDefaultAsync();
			return token != null;
		}
		public async Task SaveAccessToken(TokenResponsePayload token, string realmId)
		{
			var existingToken = await ClientDbContext.QuickBooksTokens.FirstOrDefaultAsync(t => t.RealmId == realmId);

			if (existingToken != null)
			{
				existingToken.AccessToken = token.AccessToken;
				existingToken.RefreshToken = token.RefreshToken;
				existingToken.ExpiryTime = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().AddSeconds(token.ExpiresIn);
			}
			else
			{
				ClientDbContext.QuickBooksTokens.Add(new QuickBooksToken
				{
					Id = Guid.NewGuid(),
					RealmId = realmId,
					AccessToken = token.AccessToken,
					RefreshToken = token.RefreshToken,
					ExpiryTime = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().AddSeconds(token.ExpiresIn)
				});
			}

			await ClientDbContext.SaveChangesAsync();
		}

		public async Task SyncQBClassAsync(bool isFirstRun, int daysLookupFilter, SignalRMessageModel signalRMessageModel)
		{
			this.QuickBooksToken = await ClientDbContext.QuickBooksTokens.FirstOrDefaultAsync();
			this.signalRMessageModel = signalRMessageModel;

			if (this.signalRMessageModel != null && this.signalRMessageModel.NotifyOnStart)
			{
				this.signalRMessageModel.NotificationId = await this.notificationService.SendDataSyncStartedNotification(this.signalRMessageModel);
			}

			daysLookupFilter = daysLookupFilter * -1;
			await this.SyncQBClassAsync(isFirstRun, daysLookupFilter);
			await SendQBSyncStatusNotification(completed: true);
			await SyncProposals(signalRMessageModel);
			await SyncProjectTotals(signalRMessageModel);
			await SendCompletedDataSyncStatusNotification();
		}

		public async Task SyncQBCustomerAsync(bool isFirstRun, int daysLookupFilter, SignalRMessageModel signalRMessageModel)
		{
			this.signalRMessageModel = signalRMessageModel;
			this.QuickBooksToken = await ClientDbContext.QuickBooksTokens.FirstOrDefaultAsync();

			if (this.signalRMessageModel != null && this.signalRMessageModel.NotifyOnStart)
			{
				this.signalRMessageModel.NotificationId = await this.notificationService.SendDataSyncStartedNotification(this.signalRMessageModel);
			}
			await SyncQBCustomerAsync(isFirstRun, daysLookupFilter);
			await SendCompletedDataSyncStatusNotification();
		}

		#region Private

		private async Task<List<T>> GetQBEntityAsync<T>(QuickBooksToken qbToken, string transactionType, string? condition = null, bool isFirstRun = true, int daysLookupFilter = -1)
	where T : IQBBaseEntity
		{
			var results = new List<T>();
			var startPosition = 1;
			var maxResults = 1000;
			var morePages = true;
			var txnDateFilter = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().AddDays(daysLookupFilter);

			while (morePages)
			{
				var query = $"SELECT * FROM {transactionType}";

				if (condition != null)
				{
					if (isFirstRun)
						query += $" AND {condition}";

					query += $" WHERE {condition}";
				}

				query += $" STARTPOSITION {startPosition} MAXRESULTS {maxResults}";

				var apiQueryResponse = await FetchQbApiAsync(query, typeof(T), qbToken) as QBApiResponseModel<T>;

				var qbEntity = apiQueryResponse?.QBQueryResponse?.GetTransactionData(transactionType);
				if (qbEntity?.Count > 0)
				{
					results.AddRange(qbEntity);
					startPosition += maxResults;
				}
				else
				{
					morePages = false;
				}
			}

			if (!isFirstRun)
				return results
					.Where(t => t?.MetaData?.LastUpdatedTime >= txnDateFilter)
					.ToList();
			else
				return results;

		}


		/// <summary>
		/// Run data sync for Quickbooks
		/// </summary>
		private async Task SyncQuickBooksAsync(SysDataSyncSetting qbDataSyncSetting)
		{
			var isFirstRun = qbDataSyncSetting?.IsFirstRun ?? false;
			var daysLookupFilter = (-1 * qbDataSyncSetting?.NumberOfDaysLookup) ?? -1;

			await SendQBSyncStatusNotification();
			await SyncQBAccountAsync(isFirstRun, daysLookupFilter);
			await SyncQBVendorAsync(isFirstRun, daysLookupFilter);
			await SyncQBClassAsync(isFirstRun, daysLookupFilter);
			await SyncQBCustomerAsync(isFirstRun, daysLookupFilter);
			await SyncQBItemAsync(isFirstRun, daysLookupFilter);
			await SyncQBTransactionsAsync(true, daysLookupFilter);
			await UpdateSysDataSyncSetting();
			await SendQBSyncStatusNotification(completed:true);
		}
		private async Task SendQBSyncStatusNotification(string? entityName = null, bool processing = false, bool done = false, bool completed = false, int? dataCount = null)
		{
			var messageQueue = new MessageQueue
			{
				StartMessage = "Downloading QuickBooks Data",
				EntityName = entityName,
				Processing = processing,
				Done = done,
				Completed = completed,
				ActionName = "Downloading",
				CompletionMessage = "Successfully downloaded QuickBooks Data",
				DataCount = dataCount
			};
			await SendMessageQueue(messageQueue);

		}
		private async Task SendActiveJobsStatusNotification(string? projectName = null, bool processing = false, bool done = false, bool completed = false)
		{
			var messageQueue = new MessageQueue
			{
				StartMessage = "Syncing Active Jobs",
				EntityName = projectName,
				Processing = processing,
				Done = done,
				Completed = completed,
				ActionName = "Syncing",
				CompletionMessage = "Successfully synced active jobs"
			};
			await SendMessageQueue(messageQueue);

		}
		private async Task SendProposalSyncStatusNotification(string? projectName = null, bool processing = false, bool done = false, bool completed = false)
		{
			var messageQueue = new MessageQueue
			{
				StartMessage = "Syncing Proposals",
				EntityName = projectName,
				Processing = processing,
				Done = done,
				Completed = completed,
				ActionName = "Syncing",
				CompletionMessage = "Successfully synced proposals"
			};
			await SendMessageQueue(messageQueue);

		}
		private async Task SendProjectTotalsSyncStatusNotification(string? projectName = null, bool processing = false, bool done = false, bool completed = false)
		{
			var messageQueue = new MessageQueue
			{
				StartMessage = "Syncing Project Totals",
				EntityName = projectName,
				Processing = processing,
				Done = done,
				Completed = completed,
				ActionName = "Updating",
				CompletionMessage = "Successfully synced project totals"
			};
			
			await SendMessageQueue(messageQueue);

		}
		private async Task SendCompletedDataSyncStatusNotification()
		{
			var message = $"\n💯 *Data Sync Completed Successfully*\n";

			if(this.signalRMessageModel != null && this.signalRMessageModel.NotificationId != null)
			{
				var userNotification = await ClientDbContext.UserNotifications
					.FirstOrDefaultAsync(n => n.Id == this.signalRMessageModel.NotificationId);

				ClientDbContext.UserNotifications.Remove(userNotification);
				await ClientDbContext.SaveChangesAsync();
			}

			await SendNotification(message);
		}
		private async Task SendMessageQueue(MessageQueue messageQueue)
		{
			var header = $"🔄 *{messageQueue.StartMessage}*";
			if (messageQueue.EntityName == null && messageQueue.Processing == false && messageQueue.Done == false && messageQueue.Completed == false)
			{
				await SendNotification(header);
			}
			else
			{
				var message = $"   🔄 *{messageQueue.ActionName} {messageQueue.EntityName}*";
				if (messageQueue.Processing)
				{
					await SendNotification(message);
				}
				else if (messageQueue.Done)
				{
					var updatedMessage = $"   ✔️*Done {messageQueue.EntityName} ({messageQueue.DataCount?.ToString() ?? "0"})*";
					await UpdateNotification(message, updatedMessage);
				}
				else if (messageQueue.Completed)
				{
					await UpdateNotification(header, $"✔️ *{messageQueue.StartMessage}*");
					await SendNotification($"\n{messageQueue.CompletionMessage}!\n");
				}
			}
		}


		#region QuickBooks Sync Methods

		private async Task UpdateSysDataSyncSetting()
		{
			var sysDataSyncSetting = await ClientDbContext.SysDataSyncSettings
				.FirstOrDefaultAsync(s => s.DataSyncName == "Quickbooks Sync");

			if (sysDataSyncSetting != null)
			{
				sysDataSyncSetting.IsFirstRun = false;
				sysDataSyncSetting.DateLastRun = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			}

			await ClientDbContext.SaveChangesAsync();
		}

		/// <summary>
		/// Sync quickbooks accounts data
		/// </summary>
		private async Task SyncQBAccountAsync(bool isFirstRun, int daysLookupFilter)
		{
			var entityName = "Account";
			await SendQBSyncStatusNotification(entityName: entityName, processing:true);

			var qbAccounts = new List<Qbaccount>();
			var qbFetchAccounts = await this.GetQBEntityAsync<QBAccountModel>(this.QuickBooksToken, entityName, isFirstRun: isFirstRun, daysLookupFilter: daysLookupFilter);
			var qbAccountDatasFromDb = await ClientDbContext.Qbaccounts
				.ToListAsync();
			var qbAccountDict = qbAccountDatasFromDb
				.Where(a => a.ListId != null)
				.ToDictionary(a => a.ListId!);

			foreach (var account in qbFetchAccounts)
			{
				var isExist = true;
				var qbAccount = qbAccountDict.GetValueOrDefault(account.Id);

				if (qbAccount == null)
				{
					qbAccount = new Qbaccount { Id = Guid.NewGuid() };
					isExist = false;
				}

				qbAccount.Name = account?.Name;
				qbAccount.AccountNumber = account?.AcctNum;
				qbAccount.ParentId = account?.ParentRef?.Value;
				qbAccount.IsSubAccount = account?.SubAccount;
				qbAccount.FullyQualifiedName = account?.FullyQualifiedName;
				qbAccount.AccountType = account?.AccountType;
				qbAccount.DetailType = account?.AccountSubType;
				qbAccount.CreatedBy = "QUICKBOOKS SYNC";
				qbAccount.UpdatedBy = "QUICKBOOKS SYNC";
				qbAccount.ListId = account?.Id;
				qbAccount.TimeCreated = account?.MetaData?.CreateTime;
				qbAccount.TimeModified = account?.MetaData?.LastUpdatedTime;

				if (isExist)
					ClientDbContext.Qbaccounts.Update(qbAccount);
				else
					qbAccounts.Add(qbAccount);
			}

			ClientDbContext.Qbaccounts.AddRange(qbAccounts);
			await ClientDbContext.SaveChangesAsync();

			await SendQBSyncStatusNotification(entityName: entityName, done: true, dataCount: qbFetchAccounts.Count);
		}

		/// <summary>
		/// Sync quickbooks vendors data
		/// </summary>
		private async Task SyncQBVendorAsync(bool isFirstRun, int daysLookupFilter)
		{
			var entityName = "Vendor";
			await SendQBSyncStatusNotification(entityName: entityName, processing: true);
			var qbVendors = new List<Qbvendor>();
			var qbFetchVendors = await this.GetQBEntityAsync<QBVendorModel>(this.QuickBooksToken, "Vendor", isFirstRun: isFirstRun, daysLookupFilter: daysLookupFilter);
			var qbVendorDatasFromDb = await ClientDbContext.Qbvendors
				.ToListAsync();
			var qbVendorDict = qbVendorDatasFromDb
				.Where(a => a.ListId != null)
				.ToDictionary(a => a.ListId!);

			foreach (var vendor in qbFetchVendors)
			{
				var isExist = true;
				var qbVendor = qbVendorDict.GetValueOrDefault(vendor.Id);

				if (qbVendor == null)
				{
					qbVendor = new Qbvendor { Id = Guid.NewGuid() };
					isExist = false;
				}

				qbVendor.ListId = vendor?.Id ?? string.Empty;
				qbVendor.DisplayName = vendor?.DisplayName;
				qbVendor.CompanyName = vendor?.CompanyName;
				qbVendor.TimeCreated = vendor?.MetaData?.CreateTime;
				qbVendor.TimeModified = vendor?.MetaData?.LastUpdatedTime;
				qbVendor.CreatedBy = "QUICKBOOKS SYNC";
				qbVendor.UpdatedBy = "QUICKBOOKS SYNC";

				if (isExist)
					ClientDbContext.Qbvendors.Update(qbVendor);
				else
					qbVendors.Add(qbVendor);
			}

			ClientDbContext.Qbvendors.AddRange(qbVendors);
			await ClientDbContext.SaveChangesAsync();
			await SendQBSyncStatusNotification(entityName: entityName, done: true, dataCount: qbFetchVendors.Count);
		}

		/// <summary>
		/// Sync quickbooks classes data
		/// </summary>
		private async Task SyncQBClassAsync(bool isFirstRun, int daysLookupFilter)
		{
			var entityName = "Class";
			await SendQBSyncStatusNotification(entityName: entityName, processing: true);
			var qbClasses = new List<Qbclass>();
			var qbFetchClasses = await this.GetQBEntityAsync<QBClassModel>(this.QuickBooksToken, "Class", isFirstRun: isFirstRun, daysLookupFilter: daysLookupFilter);
			var qbClassDatasFromDb = await ClientDbContext.Qbclasses
				.AsNoTracking()
				.ToListAsync();

			var qbClassDict = qbClassDatasFromDb
				.Where(a => a.ListId != null && a.ListId != "NonQBOProject")
				.ToDictionary(a => a.ListId!);

			//var defaultCompanyOwner = await clientDbContext.Users.Where(u => u.RoleId == (int)Roles.CompanyOwner).OrderBy(u=> u.Id).FirstOrDefaultAsync();
			var projectSupervisors = new List<ProjectSupervisor>();

			foreach (var qbClass in qbFetchClasses)
			{
				var isExist = true;
				var qbClassEntity = qbClassDict.GetValueOrDefault(qbClass.Id);

				if (qbClassEntity == null)
				{
					qbClassEntity = new Qbclass { Id = Guid.NewGuid() };
					isExist = false;
				}

				qbClassEntity.ListId = qbClass?.Id;
				qbClassEntity.Name = qbClass?.Name;
				qbClassEntity.SubClass = qbClass?.SubClass;
				qbClassEntity.FullyQualifiedName = qbClass?.FullyQualifiedName;
				qbClassEntity.TimeCreated = qbClass?.MetaData?.CreateTime;
				qbClassEntity.TimeModified = qbClass?.MetaData?.LastUpdatedTime;
				qbClassEntity.CreatedBy = "QUICKBOOKS SYNC";
				qbClassEntity.UpdatedBy = "QUICKBOOKS SYNC";
				qbClassEntity.ActiveJobs = true;
				qbClassEntity.IsActive = true;
				//qbClassEntity.IsActive = qbClass?.Active ?? false;

				if (isExist)
					ClientDbContext.Qbclasses.Update(qbClassEntity);
				else
				{
					qbClasses.Add(qbClassEntity);

					////add default supervisor here
					//projectSupervisors.Add(new ProjectSupervisor
					//{
					//	ProjectId = qbClassEntity.Id,
					//	SupervisorId = defaultCompanyOwner.Id,
					//	DateAssigned = DateOnly.FromDateTime(TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()),
					//	SupervisorTypeId = (int)SupervisorType.ProjectManager
					//});
				}

			}

			ClientDbContext.Qbclasses.AddRange(qbClasses);

			if (projectSupervisors.Count > 0)
				ClientDbContext.ProjectSupervisors.AddRange(projectSupervisors);

			await ClientDbContext.SaveChangesAsync();
			await SendQBSyncStatusNotification(entityName: entityName, done: true, dataCount: qbFetchClasses.Count);
		}

		/// <summary>
		/// Sync quickbooks customers data
		/// </summary>
		private async Task SyncQBCustomerAsync(bool isFirstRun, int daysLookupFilter)
		{
			var entityName = "Customer";
			await SendQBSyncStatusNotification(entityName: entityName, processing: true);
			var qbCustomers = new List<Qbcustomer>();
			var qbFetchCustomers = await this.GetQBEntityAsync<QBCustomerModel>(this.QuickBooksToken, "Customer", isFirstRun: isFirstRun, daysLookupFilter: daysLookupFilter);
			var qbCustomerDatasFromDb = await ClientDbContext.Qbcustomers
				.ToListAsync();
			var qbCustomerDict = qbCustomerDatasFromDb
				.Where(a => a.ListId != null && a.ListId != "NonQBOCustomer")
				.ToDictionary(a => a.ListId!);

			foreach (var customer in qbFetchCustomers)
			{
				var isExist = true;
				var qbCustomerEntity = qbCustomerDict.GetValueOrDefault(customer.Id);

				if (qbCustomerEntity == null)
				{
					qbCustomerEntity = new Qbcustomer { Id = Guid.NewGuid() };
					isExist = false;
				}

				var address = customer?.BillAddr;

				qbCustomerEntity.ListId = customer?.Id ?? string.Empty;
				qbCustomerEntity.Name = customer?.GivenName;
				qbCustomerEntity.LastName = customer?.FamilyName;
				qbCustomerEntity.FullName = customer?.FullyQualifiedName;
				qbCustomerEntity.CompanyName = customer?.CompanyName;
				qbCustomerEntity.Address = $"{address?.Line1} {address?.City} {address?.CountrySubDivisionCode}";
				qbCustomerEntity.Balance = customer?.Balance;
				qbCustomerEntity.Currency = customer?.CurrencyRef?.Value;
				qbCustomerEntity.Phone = customer?.PrimaryPhone?.FreeFormNumber;
				qbCustomerEntity.Email = customer?.PrimaryEmailAddr?.Address;
				qbCustomerEntity.IsActive = customer?.Active;
				qbCustomerEntity.Level = customer?.Level;
				qbCustomerEntity.ParentId = customer?.ParentRef?.Value;
				qbCustomerEntity.TimeCreated = customer?.MetaData?.CreateTime;
				qbCustomerEntity.TimeModified = customer?.MetaData?.LastUpdatedTime;
				qbCustomerEntity.CreatedBy = "QUICKBOOKS SYNC";
				qbCustomerEntity.UpdatedBy = "QUICKBOOKS SYNC";

				if (isExist)
					ClientDbContext.Qbcustomers.Update(qbCustomerEntity);
				else
					qbCustomers.Add(qbCustomerEntity);
			}

			ClientDbContext.Qbcustomers.AddRange(qbCustomers);
			await ClientDbContext.SaveChangesAsync();
			await SendQBSyncStatusNotification(entityName: entityName, done: true, dataCount: qbFetchCustomers.Count);
		}

		/// <summary>
		/// Sync quickbooks items data
		/// </summary>
		private async Task SyncQBItemAsync(bool isFirstRun, int daysLookupFilter)
		{
			var entityName = "Item";
			await SendQBSyncStatusNotification(entityName: entityName, processing: true);
			var qbItems = new List<Qbitem>();
			var qbFetchItems = await this.GetQBEntityAsync<QBItemModel>(this.QuickBooksToken, "Item", isFirstRun: isFirstRun, daysLookupFilter: daysLookupFilter);
			var qbItemDatasFromDb = await ClientDbContext.Qbitems
				.ToListAsync();
			var qbItemDict = qbItemDatasFromDb
				.Where(a => a.ListId != null)
				.ToDictionary(a => a.ListId!);

			foreach (var item in qbFetchItems)
			{
				var isExist = true;
				var qbItemEntity = qbItemDict.GetValueOrDefault(item.Id);

				if (qbItemEntity == null)
				{
					qbItemEntity = new Qbitem { Id = Guid.NewGuid() };
					isExist = false;
				}

				qbItemEntity.ListId = item?.Id ?? string.Empty;
				qbItemEntity.Name = item?.Name;
				qbItemEntity.FullName = item?.FullyQualifiedName;
				qbItemEntity.ExpenseAccountId = item?.ExpenseAccountRef?.Value;
				qbItemEntity.IsActive = item?.Active;
				qbItemEntity.Type = item?.Type;
				qbItemEntity.TimeCreated = item?.MetaData?.CreateTime;
				qbItemEntity.TimeModified = item?.MetaData?.LastUpdatedTime;
				qbItemEntity.CreatedBy = "QUICKBOOKS SYNC";
				qbItemEntity.UpdatedBy = "QUICKBOOKS SYNC";

				if (isExist)
					ClientDbContext.Qbitems.Update(qbItemEntity);
				else
					qbItems.Add(qbItemEntity);
			}

			ClientDbContext.Qbitems.AddRange(qbItems);
			await ClientDbContext.SaveChangesAsync();
			await SendQBSyncStatusNotification(entityName: entityName, done: true, dataCount: qbFetchItems.Count);
		}

		/// <summary>
		/// Sync quickbooks transactions data
		/// </summary>
		private async Task SyncQBTransactionsAsync(bool isFirstRun, int daysLookupFilter)
		{
			dbAccounts = await ClientDbContext.Qbaccounts.ToListAsync();
			dbVendors = await ClientDbContext.Qbvendors.ToListAsync();
			dbClasses = await ClientDbContext.Qbclasses.ToListAsync();
			dbCustomers = await ClientDbContext.Qbcustomers.ToListAsync();
			deleteQbTransactions = true;

			await QBTransactionProcessorAsync<QBBillModel, QBBillLineModel>("Bill", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBBillPaymentModel, QBBillPaymentPaymentLineModel>("BillPayment", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBPurchaseModel, QBPurchaseLineModel>("Purchase", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBCreditMemoModel, QBCreditMemoLineModel>("CreditMemo", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBDepositModel, QBDepositLineModel>("Deposit", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBInvoiceModel, QBInvoiceLineModel>("Invoice", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBJournalEntryModel, QBJournalLineModel>("JournalEntry", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBPaymentModel, QBPaymentLineModel>("Payment", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBSalesReceiptModel, QBSalesReceiptLineModel>("SalesReceipt", isFirstRun, daysLookupFilter);
			await QBTransactionProcessorAsync<QBTransferModel, QBTransferLineModel>("Transfer", isFirstRun, daysLookupFilter);
		}

		/// <summary>
		/// Retrieve and map quickbooks data and save to database.
		/// </summary>
		private async Task QBTransactionProcessorAsync<TRoot, TLine>(string qbTxnEntity, bool isFirstRun, int daysLookupFilter)
			where TRoot : QBTransactionEntity<TLine>, IQBBaseEntity
			where TLine : IQBBaseLineEntity
		{

			await SendQBSyncStatusNotification(entityName: qbTxnEntity, processing: true);

			var qbFetchItemEntityData = new List<QBItemModel>();
			var qbFullTransactions = new List<Qbtransaction>();
			var qbFetchEntityData = await this.GetQBEntityAsync<TRoot>(this.QuickBooksToken, qbTxnEntity, isFirstRun: isFirstRun, daysLookupFilter: daysLookupFilter);
			var txnIds = qbFetchEntityData.Select(t => t?.Id).Where(id => id != null).ToList();
			var itemDependentTxnTypes = new HashSet<string> { "CreditMemo", "Invoice", "SalesReceipt" };

			if (itemDependentTxnTypes.Contains(qbTxnEntity))
			{
				qbFetchItemEntityData = await this.GetQBEntityAsync<QBItemModel>(this.QuickBooksToken, "Item", isFirstRun: true);
			}

			foreach (var entity in qbFetchEntityData)
			{
				if (entity is QBBillModel billModel)
					qbFullTransactions.AddRange(TransformQBEntity(billModel));

				if (entity is QBBillPaymentModel billPaymentModel)
					qbFullTransactions.AddRange(TransformQBEntity(billPaymentModel));

				if (entity is QBPurchaseModel purchaseModel)
					qbFullTransactions.AddRange(TransformQBEntity(purchaseModel));

				if (entity is QBCreditMemoModel creditMemoModel)
					qbFullTransactions.AddRange(TransformQBEntity(creditMemoModel, qbFetchItemEntityData));

				if (entity is QBDepositModel depositModel)
					qbFullTransactions.AddRange(TransformQBEntity(depositModel));

				if (entity is QBInvoiceModel invoiceModel)
					qbFullTransactions.AddRange(TransformQBEntity(invoiceModel, qbFetchItemEntityData));

				if (entity is QBJournalEntryModel journalEntryModel)
					qbFullTransactions.AddRange(TransformQBEntity(journalEntryModel));

				if (entity is QBPaymentModel paymentModel)
				{
					var transactions = TransformQBEntity(paymentModel);
					var entityLine = paymentModel?.Line;

					var lineLinkedTxnId = paymentModel?.Line?.FirstOrDefault()
						?.LinkedTxn?.FirstOrDefault()?.TxnId;
					if (!string.IsNullOrEmpty(lineLinkedTxnId))
					{
						var txnData = await ClientDbContext.Qbtransactions
							.FirstOrDefaultAsync(t => t.TxnId == lineLinkedTxnId);

						transactions.ForEach(t =>
						{
							t.Location = txnData?.Location;
						});
					}

					qbFullTransactions.AddRange(transactions);
				}

				if (entity is QBSalesReceiptModel salesReceiptModel)
					qbFullTransactions.AddRange(TransformQBEntity(salesReceiptModel, qbFetchItemEntityData));

				if (entity is QBTransferModel transferModel)
					qbFullTransactions.AddRange(TransformQBEntity(transferModel));
			}

			await SaveTransactionsByBatch(qbFullTransactions, qbTxnEntity);
			await SendQBSyncStatusNotification(entityName: qbTxnEntity, done: true, dataCount: qbFullTransactions.Count);
		}

		private async Task SaveTransactionsByBatch(List<Qbtransaction> transactions, string qbTxnEntity)
		{
			var bulkConfig = new BulkConfig
			{
				BatchSize = 5000,
				PreserveInsertOrder = true,
				SetOutputIdentity = false,
				BulkCopyTimeout = 300
			};

			using var transaction = await ClientDbContext.Database.BeginTransactionAsync();

			if (deleteQbTransactions)
			{
				await ClientDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [Qbtransactions]");
				deleteQbTransactions = false;
			}

			await ClientDbContext.BulkInsertAsync(transactions, bulkConfig);
			await transaction.CommitAsync();
		}

		#endregion

		/*
		 * Quickbooks Entity Data Transformation to Transaction || Quickbooks Entity Data Mapping
		 */

		#region Bill Transformation

		/// <summary>
		/// Transform each quickbooks bill records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBBillModel entity)
		{
			var transactions = new List<Qbtransaction>();
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == entity?.APAccountRef?.Value);
			var rootVendorId = dbVendors.FirstOrDefault(a => a.ListId == entity?.VendorRef?.Value)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.VendorId = rootVendorId == Guid.Empty ? null : rootVendorId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = "Bill";
			rootTransaction.Name = entity?.VendorRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					var entityLine = TransformQBEntity_Line(rootTransaction, line);
					transactions.Add(entityLine);
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks bill lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBBillLineModel entityLine)
		{
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == entityLine?.AccountBasedExpenseLineDetail?.AccountRef?.Value);
			var lineClassId = dbClasses.FirstOrDefault(a => a.ListId == entityLine?.AccountBasedExpenseLineDetail?.ClassRef?.Value)?.Id;
			var lineCustomerId = dbCustomers.FirstOrDefault(a => a.ListId == entityLine?.AccountBasedExpenseLineDetail?.CustomerRef?.Value)?.Id;
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.VendorId = rootTransaction.VendorId;
			lineTransaction.ClassId = lineClassId == Guid.Empty ? null : lineClassId;
			lineTransaction.CustomerId = lineCustomerId == Guid.Empty ? null : lineCustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region BillPayment Transformation

		/// <summary>
		/// Transform each quickbooks bill payment records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBBillPaymentModel entity)
		{
			var transactions = new List<Qbtransaction>();
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == "68"); // Set to default AccountsPayable
			var rootVendorId = dbVendors.FirstOrDefault(v => v.ListId == entity?.VendorRef?.Value)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.VendorId = rootVendorId == Guid.Empty ? null : rootVendorId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = $"Bill Payment ({entity?.PayType})";
			rootTransaction.Name = entity?.VendorRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					var entityLine = TransformQBEntity_Line(rootTransaction, line, entity);
					transactions.Add(entityLine);
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks bill lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBBillPaymentPaymentLineModel entityLine, QBBillPaymentModel entity)
		{
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == entity?.CheckPayment?.BankAccountRef?.Value);
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.VendorId = rootTransaction.VendorId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region Purchase Transformation

		/// <summary>
		/// Transform each quickbooks purchase records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBPurchaseModel entity)
		{
			var transactions = new List<Qbtransaction>();
			var entityRef = entity?.EntityRef;
			(string rootVendorRefId, string rootCustomerRefId) = entityRef?.Type?.ToUpper() switch
			{
				"VENDOR" => (entityRef.Value, ""),
				"CUSTOMER" => ("", entityRef.Value),
				_ => ("", "")
			};
			var txnType = entity?.PaymentType switch
			{
				"Check" => "Check",
				"Cash" => !string.IsNullOrEmpty(entity?.DocNumber) && entity.DocNumber.Contains("onlline") ? "Cash Expense" : "Expense",
				_ => entity?.PaymentType
			};
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == entity?.AccountRef?.Value);
			var rootVendorId = dbVendors.FirstOrDefault(a => a.ListId == rootVendorRefId)?.Id;
			var rootCustomerId = dbCustomers.FirstOrDefault(a => a.ListId == rootCustomerRefId)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? entity?.TotalAmt : (-1 * entity?.TotalAmt);
			rootTransaction.VendorId = rootVendorId == Guid.Empty ? null : rootVendorId;
			rootTransaction.CustomerId = rootCustomerId == Guid.Empty ? null : rootCustomerId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = txnType;
			rootTransaction.Memo = entity?.PrivateNote;
			rootTransaction.Name = entity?.EntityRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.Status = entity?.Status;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					var entityLine = TransformQBEntity_Line(rootTransaction, line);
					transactions.Add(entityLine);
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks purchase lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBPurchaseLineModel entityLine)
		{
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == entityLine?.AccountBasedExpenseLineDetail?.AccountRef?.Value);
			var lineClassId = dbClasses.FirstOrDefault(a => a.ListId == entityLine?.AccountBasedExpenseLineDetail?.ClassRef?.Value)?.Id;
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? entityLine?.Amount : (-1 * entityLine?.Amount);
			lineTransaction.VendorId = rootTransaction.VendorId;
			lineTransaction.ClassId = lineClassId == Guid.Empty ? null : lineClassId;
			lineTransaction.CustomerId = rootTransaction.CustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.Status = rootTransaction.Status;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region Credit Memo Transformation

		/// <summary>
		/// Transform each quickbooks credit memo records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBCreditMemoModel entity, List<QBItemModel> qbItemsFetched)
		{
			var transactions = new List<Qbtransaction>();
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == "66"); // Set to default AccountsReceivable
			var rootClassId = dbClasses.FirstOrDefault(c => c.ListId == entity?.ClassRef?.Value)?.Id;
			var rootCustomerId = dbCustomers.FirstOrDefault(c => c.ListId == entity?.CustomerRef?.Value)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.ClassId = rootClassId == Guid.Empty ? null : rootClassId;
			rootTransaction.CustomerId = rootCustomerId == Guid.Empty ? null : rootCustomerId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = "CreditMemo";
			rootTransaction.Name = entity?.CustomerRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					if (!string.IsNullOrEmpty(line?.Id))
					{
						var entityLine = TransformQBEntity_Line(rootTransaction, line, qbItemsFetched);
						transactions.Add(entityLine);
					}
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks credit memo lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBCreditMemoLineModel entityLine, List<QBItemModel> qbItemsFetched)
		{
			var item = qbItemsFetched.SingleOrDefault(i => i.Id == entityLine?.SalesItemLineDetail?.ItemRef?.Value);
			var itemRefId = item?.IncomeAccountRef?.Value ?? "307"; // If Item is null set to 307(Uncategorized Income) account
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == itemRefId);
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.ClassId = rootTransaction.ClassId;
			lineTransaction.CustomerId = rootTransaction.CustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region Deposit Transformation

		/// <summary>
		/// Transform each quickbooks deposit records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBDepositModel entity)
		{
			var transaction = new List<Qbtransaction>();

			var entityRef = entity?.Line[0]?.DepositLineDetail?.Entity;
			(string rootVendorRefId, string rootCustomerRefId) = entityRef?.Type?.ToUpper() switch
			{
				"VENDOR" => (entityRef.Value, ""),
				"CUSTOMER" => ("", entityRef.Value),
				_ => ("", "")
			};
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == entity?.DepositToAccountRef?.Value);
			var rootVendorId = dbVendors.FirstOrDefault(a => a.ListId == rootVendorRefId)?.Id;
			var rootCustomerId = dbCustomers.FirstOrDefault(a => a.ListId == rootCustomerRefId)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.VendorId = rootVendorId == Guid.Empty ? null : rootVendorId;
			rootTransaction.CustomerId = rootCustomerId == Guid.Empty ? null : rootCustomerId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = "Deposit";
			rootTransaction.Name = entity?.Line[0]?.DepositLineDetail?.Entity?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.CreateTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transaction.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					var entityLine = TransformQBEntity_Line(rootTransaction, line);
					transaction.Add(entityLine);
				}
			}

			return transaction;
		}

		/// <summary>
		/// Transform each quickbooks deposit lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBDepositLineModel entityLine)
		{
			var entityRef = entityLine?.DepositLineDetail?.Entity;
			(string lineVendorRefId, string lineCustomerRefId) = entityRef?.Type?.ToUpper() switch
			{
				"VENDOR" => (entityRef.Value, ""),
				"CUSTOMER" => ("", entityRef.Value),
				_ => ("", "")
			};
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == entityLine?.DepositLineDetail?.AccountRef?.Value);
			var lineVendorId = dbVendors.FirstOrDefault(a => a.ListId == lineVendorRefId)?.Id;
			var lineClassId = dbClasses.FirstOrDefault(a => a.ListId == entityLine?.DepositLineDetail?.ClassRef?.Value)?.Id;
			var lineCustomerId = dbCustomers.FirstOrDefault(a => a.ListId == lineCustomerRefId)?.Id;

			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.VendorId = lineVendorId == Guid.Empty ? null : lineVendorId;
			lineTransaction.ClassId = lineClassId == Guid.Empty ? null : lineClassId;
			lineTransaction.CustomerId = lineCustomerId == Guid.Empty ? null : lineCustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = entityLine?.DepositLineDetail?.Entity?.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region Invoice Transformation

		/// <summary>
		/// Transform each quickbooks invoice records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBInvoiceModel entity, List<QBItemModel> qbItemsFetched)
		{
			var transactions = new List<Qbtransaction>();
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == "724"); // Set to default AccountsReceivable
			var rootClassId = dbClasses.FirstOrDefault(a => a.ListId == entity?.ClassRef?.Value)?.Id;
			var rootCustomerId = dbCustomers.FirstOrDefault(a => a.ListId == entity?.CustomerRef?.Value)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.ClassId = rootClassId == Guid.Empty ? null : rootClassId;
			rootTransaction.CustomerId = rootCustomerId == Guid.Empty ? null : rootCustomerId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = "Invoice";
			rootTransaction.Name = entity?.CustomerRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					if (!string.IsNullOrEmpty(line?.Id))
					{
						var entityLine = TransformQBEntity_Line(rootTransaction, line, qbItemsFetched);
						transactions.Add(entityLine);
					}
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks invoice lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBInvoiceLineModel entityLine, List<QBItemModel> qbItemsFetched)
		{
			var item = qbItemsFetched.SingleOrDefault(i => i.Id == entityLine?.SalesItemLineDetail?.ItemRef?.Value);
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == item?.IncomeAccountRef?.Value);
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.ClassId = rootTransaction.ClassId;
			lineTransaction.CustomerId = rootTransaction.CustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region Journal Entry Transformation

		/// <summary>
		/// Transform each quickbooks journal entry records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBJournalEntryModel entity)
		{
			var transactions = new List<Qbtransaction>();
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					var entityLine = TransformQBEntity_Line(rootTransaction, line);
					transactions.Add(entityLine);
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks journal entry lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBJournalLineModel entityLine)
		{
			var entityRef = entityLine?.JournalEntryLineDetail?.Entity?.EntityRef;
			(string rootVendorRefId, string rootCustomerRefId) = entityRef?.Type?.ToUpper() switch
			{
				"VENDOR" => (entityRef.Value, ""),
				"CUSTOMER" => ("", entityRef.Value),
				_ => ("", "")
			};
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == entityLine?.JournalEntryLineDetail?.AccountRef?.Value);
			var lineVendorId = dbVendors.FirstOrDefault(v => v.ListId == rootVendorRefId)?.Id;
			var lineClassId = dbClasses.FirstOrDefault(c => c.ListId == entityLine?.JournalEntryLineDetail?.ClassRef?.Value)?.Id;
			var lineCustomerId = dbCustomers.FirstOrDefault(c => c.ListId == rootCustomerRefId)?.Id;
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.VendorId = lineVendorId == Guid.Empty ? null : lineVendorId;
			lineTransaction.ClassId = lineClassId == Guid.Empty ? null : lineClassId;
			lineTransaction.CustomerId = lineCustomerId == Guid.Empty ? null : lineCustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = "Journal Entry";
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = entityLine?.JournalEntryLineDetail?.Entity?.EntityRef?.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.Location = entityLine?.JournalEntryLineDetail?.DepartmentRef?.Name;

			return lineTransaction;
		}

		#endregion

		#region Payment Transformation

		/// <summary>
		/// Transform each quickbooks payment records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBPaymentModel entity)
		{
			var transactions = new List<Qbtransaction>();
			var rootAccount = dbAccounts.FirstOrDefault(a => a.DetailType == "AccountsReceivable"); // Set to default AccountsReceivable
			var rootCustomerId = dbCustomers.FirstOrDefault(c => c.ListId == entity?.CustomerRef?.Value)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.CustomerId = rootCustomerId == Guid.Empty ? null : rootCustomerId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.PaymentRefNum;
			rootTransaction.TxnType = "Payment";
			rootTransaction.Memo = entity?.PrivateNote;
			rootTransaction.Name = entity?.CustomerRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					var entityLine = TransformQBEntity_Line(rootTransaction, line, entity);
					transactions.Add(entityLine);
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks payment lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBPaymentLineModel entityLine, QBPaymentModel entity)
		{
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == entity?.DepositToAccountRef?.Value);
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.CustomerId = rootTransaction.CustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = rootTransaction.Memo;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;

			return lineTransaction;
		}

		#endregion

		#region Sales Receipt Transformation

		/// <summary>
		/// Transform each quickbooks sales receipt records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBSalesReceiptModel entity, List<QBItemModel> qbItemsFetched)
		{
			var transactions = new List<Qbtransaction>();
			var rootAccount = dbAccounts.FirstOrDefault(a => a.ListId == entity?.DepositToAccountRef?.Value);
			var rootCustomerId = dbCustomers.FirstOrDefault(c => c.ListId == entity?.CustomerRef?.Value)?.Id;
			var rootTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			rootTransaction.AccountId = rootAccount?.Id == Guid.Empty ? null : rootAccount?.Id;
			rootTransaction.Amount = rootAccount?.AccountType?.Contains("Expense") == true ? (-1 * entity?.TotalAmt) : entity?.TotalAmt;
			rootTransaction.CustomerId = rootCustomerId == Guid.Empty ? null : rootCustomerId;
			rootTransaction.TxnId = entity?.Id;
			rootTransaction.TxnNumber = entity?.DocNumber;
			rootTransaction.TxnType = "Sales Receipt";
			rootTransaction.Memo = entity?.PrivateNote;
			rootTransaction.Name = entity?.CustomerRef?.Name;
			rootTransaction.Currency = entity?.CurrencyRef?.Value;
			rootTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			rootTransaction.Created = entity?.MetaData?.CreateTime;
			rootTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			rootTransaction.CreatedBy = "QUICKBOOKS SYNC";
			rootTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			rootTransaction.Location = entity?.DepartmentRef?.Name;

			transactions.Add(rootTransaction);

			if (entity?.Line != null)
			{
				foreach (var line in entity.Line)
				{
					if (!string.IsNullOrEmpty(line?.Id))
					{
						var entityLine = TransformQBEntity_Line(rootTransaction, line, qbItemsFetched);
						transactions.Add(entityLine);
					}
				}
			}

			return transactions;
		}

		/// <summary>
		/// Transform each quickbooks sales receipt lines from single record into transaction
		/// </summary>
		private Qbtransaction TransformQBEntity_Line(Qbtransaction rootTransaction, QBSalesReceiptLineModel entityLine, List<QBItemModel> qbItemsFetched)
		{
			var item = qbItemsFetched.SingleOrDefault(i => i.Id == entityLine?.SalesItemLineDetail?.ItemRef?.Value);
			var lineAccount = dbAccounts.FirstOrDefault(a => a.ListId == item?.IncomeAccountRef?.Value);
			var lineTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			lineTransaction.AccountId = lineAccount?.Id == Guid.Empty ? null : lineAccount?.Id;
			lineTransaction.Amount = lineAccount?.AccountType?.Contains("Expense") == true ? (-1 * entityLine?.Amount) : entityLine?.Amount;
			lineTransaction.CustomerId = rootTransaction.CustomerId;
			lineTransaction.TxnId = rootTransaction.TxnId;
			lineTransaction.TxnNumber = rootTransaction.TxnNumber;
			lineTransaction.TxnType = rootTransaction.TxnType;
			lineTransaction.Memo = entityLine?.Description;
			lineTransaction.Name = rootTransaction.Name;
			lineTransaction.Currency = rootTransaction.Currency;
			lineTransaction.TransactionDate = rootTransaction.TransactionDate;
			lineTransaction.Created = rootTransaction.Created;
			lineTransaction.Updated = rootTransaction.Updated;
			lineTransaction.CreatedBy = "QUICKBOOKS SYNC";
			lineTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			lineTransaction.SplitAccountId = rootTransaction.AccountId;
			lineTransaction.Location = rootTransaction.Location;

			return lineTransaction;
		}

		#endregion

		#region Transfer Transformation

		/// <summary>
		/// Transform each quickbooks transfer records into transaction
		/// </summary>
		private List<Qbtransaction> TransformQBEntity(QBTransferModel entity)
		{
			var transactions = new List<Qbtransaction>();
			var fromAccountId = dbAccounts.FirstOrDefault(a => a.ListId == entity?.FromAccountRef?.Value)?.Id;
			var toAccountId = dbAccounts.FirstOrDefault(a => a.ListId == entity?.ToAccountRef?.Value)?.Id;
			var fromTransaction = new Qbtransaction { Id = Guid.NewGuid() };

			fromTransaction.AccountId = fromAccountId == Guid.Empty ? null : fromAccountId;
			fromTransaction.Amount = (-1 * entity?.Amount);
			fromTransaction.TxnId = entity?.Id;
			fromTransaction.TxnType = "Transfer";
			fromTransaction.Memo = entity?.PrivateNote;
			fromTransaction.Currency = entity?.CurrencyRef?.Value;
			fromTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			fromTransaction.Created = entity?.MetaData?.CreateTime;
			fromTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			fromTransaction.CreatedBy = "QUICKBOOKS SYNC";
			fromTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			fromTransaction.SplitAccountId = toAccountId == Guid.Empty ? null : toAccountId;

			var toTransaction = new Qbtransaction { Id = Guid.NewGuid() };
			toTransaction.AccountId = toAccountId == Guid.Empty ? null : toAccountId;
			toTransaction.Amount = entity?.Amount;
			toTransaction.TxnId = entity?.Id;
			toTransaction.TxnType = "Transfer";
			toTransaction.Memo = entity?.PrivateNote;
			toTransaction.Currency = entity?.CurrencyRef?.Value;
			toTransaction.TransactionDate = DateTime.Parse(entity?.TxnDate ?? "0001-01-01");
			toTransaction.Created = entity?.MetaData?.CreateTime;
			toTransaction.Updated = entity?.MetaData?.LastUpdatedTime;
			toTransaction.CreatedBy = "QUICKBOOKS SYNC";
			toTransaction.UpdatedBy = "QUICKBOOKS SYNC";
			toTransaction.SplitAccountId = fromAccountId == Guid.Empty ? null : fromAccountId;

			transactions.Add(fromTransaction);
			transactions.Add(toTransaction);

			return transactions;
		}

		#endregion

		/// <summary>
		/// Fetches quickbooks data thru API
		/// </summary>
		private async Task<object> FetchQbApiAsync(string query, Type qbObject, QuickBooksToken quickbooksToken)
		{
			var token = await RefreshQuickBooksTokenAsync(quickbooksToken);
			var baseUrl = configuration.GetSection("QuickBooks").GetValue<string>("BaseUrl")
						 ?? throw new Exception("QuickBooks BaseUrl is missing.");
			var url = $"{baseUrl}/v3/company/{quickbooksToken.RealmId}/query";
			var response = await ExecuteQuickBooksRequestAsync(url, query, token.AccessToken);
			var genericType = typeof(QBApiResponseModel<>).MakeGenericType(qbObject);

			return JsonConvert.DeserializeObject(response.Content ?? "{}", genericType)
				?? genericType;
		}

		/// <summary>
		/// Fetches quickbooks token from database
		/// </summary>
		private async Task<QuickBooksToken> RefreshQuickBooksTokenAsync(QuickBooksToken token)
		{
			var currentUtcTime = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();

			if (token.ExpiryTime < currentUtcTime)
			{
				var newToken = await RefreshAccessTokenAsync(token.RefreshToken);
				token.AccessToken = newToken.AccessToken;
				token.RefreshToken = newToken.RefreshToken;
				token.ExpiryTime = currentUtcTime.AddSeconds(newToken.ExpiresIn);
				await ClientDbContext.SaveChangesAsync();
			}

			return token;
		}

		/// <summary>
		/// Fetches refresh token
		/// </summary>
		private async Task<TokenResponsePayload> RefreshAccessTokenAsync(string refreshToken)
		{
			var qbSettings = configuration.GetSection("QuickBooks");
			var tokenEndpoint = qbSettings.GetSection("TokenEndpoint").Value;
			var clientId = qbSettings.GetSection("ClientId").Value;
			var clientSecret = qbSettings.GetSection("ClientSecret").Value;
			var redirectUri = qbSettings.GetSection("RedirectUri").Value
				?? throw new InvalidOperationException("QuickBooks RedirectUri are not configured.");

			using var client = new HttpClient();
			var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(
				Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}")));

			request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
			{
				{ "grant_type", "refresh_token" },
				{ "refresh_token", refreshToken }
			});

			var response = await client.SendAsync(request);
			response.EnsureSuccessStatusCode();
			var responseContent = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<TokenResponsePayload>(responseContent)
				?? throw new NullReferenceException("QuickBooks request get refresh token returned null.");
		}

		/// <summary>
		/// Send a request to QuickBooks API.
		/// </summary>
		private async Task<RestResponse> ExecuteQuickBooksRequestAsync(string url, string query, string accessToken)
		{
			var currentUtcTime = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			using var client = new RestClient(url);
			var request = new RestRequest { Method = Method.Post };

			request.AddHeader("Authorization", $"Bearer {accessToken}");
			request.AddHeader("Accept", "application/json");
			request.AddHeader("Content-Type", "application/text");
			request.AddHeader("If-Modified-Since", currentUtcTime.ToString("yyyy-MM-ddTHH:mm:ssZ"));
			request.AddParameter("application/text", query, ParameterType.RequestBody);

			var response = await client.ExecuteAsync(request);
			if (!response.IsSuccessful)
			{
				throw new Exception($"QuickBooks API request failed: {response.StatusCode} - {response.Content}");
			}
			return response;
		}


		#endregion
		private class MessageQueue
		{
			public string StartMessage { get; set; }
			public string ActionName { get; set; }
			public string CompletionMessage { get; set; }
			public string? EntityName { get; set; }
			public int? DataCount { get; set; }
			public bool Processing { get; set; }
			public bool Done { get; set; }
			public bool Completed{ get; set; }
		}
	}
}
