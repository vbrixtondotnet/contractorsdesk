using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ContractorsDesk.Core.ApiPayloadModels;
using Pipelines.Sockets.Unofficial.Arenas;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class EstimateService : BaseService, IEstimateService
	{
		private readonly IProjectsService projectsService;
		private readonly ICacheService cacheService;
		private readonly IChangeOrderService changeOrderService;
		public EstimateService(
			IProjectsService projectsService,
			ICacheService cacheService,
			IChangeOrderService changeOrderService,
			ClientDbContext clientDataDbContext, 
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDataDbContext, masterDbContext, configuration)
		{
			this.projectsService = projectsService;
			this.cacheService = cacheService;
		}

		#region Public Methods
		public async Task<EstimateToActualDto> GetEstimateToActualAsync(Guid proposalId)
		{
			EstimateToActualDto? retval;

			retval = new EstimateToActualDto();

			var dbProposal = await this.ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == proposalId);
			if (dbProposal == null) throw new Exception("Proposal not found.");

			var projectId = dbProposal.QbclassId;

			retval.ProjectDetails = await projectsService.GetProjectShortDetailsAsync(projectId.Value);

			await SynchTransactionsToEstimates(proposalId);
			await EnsureUniqueProposalLinesAsync(proposalId);

			var dbRevisedEstimateData = await this.GetRevisedEstimateSpResult(proposalId);

			var summaryItems = new List<string> { "TOTAL COST TO DATE", "OWNER DEPOSITS", "JOB BALANCE", "TOTAL COST TO DATE", "MINIMUM REQUESTED AMOUNT" };

            //A5DA1985-1858-49E8-ABB8-558EFC382AAC is overhead category
			var overheadCategoryId = new Guid("a5da1985-1858-49e8-abb8-558efc382aac");
            dbRevisedEstimateData = dbRevisedEstimateData
				.Where(r => (r.OriginalAmount + r.CostToDate + r.RevisedAmount != 0) || summaryItems.Contains(r.Name.ToUpper()) || r.ParentEstimateCategoryId == overheadCategoryId)
				.ToList();

			await ProcessParentCategories(retval, dbRevisedEstimateData);
			await ProcessUnmappedTransactions(retval, dbRevisedEstimateData);
			await ProcessSummary(retval, dbRevisedEstimateData, summaryItems);

			return retval;
		}
		public async Task<List<EstimateCategoryDto>> GetEstimateItemsByNameAsync(string name)
		{
			var estimateCategories = await ClientDbContext.EstimateCategories
				.Where(e=> e.ParentEstimateCategoryId != null && e.Name.Trim().ToUpper().StartsWith(name.Trim().ToUpper()))
				.ToListAsync();

			return mapper.Map<List<EstimateCategoryDto>>(estimateCategories);
		}
		public async Task<List<EstimateCategoryDto>> GetEstimateCategoriesByNameAsync(string name)
		{
			var estimateCategories = await ClientDbContext.EstimateCategories
				.Where(e => e.ParentEstimateCategoryId == null && e.Name.Trim().ToUpper().StartsWith(name.Trim().ToUpper()))
				.ToListAsync();

			return mapper.Map<List<EstimateCategoryDto>>(estimateCategories);
		}
		public async Task<CostRevisionDto?> SaveRevisedEstimateAsync(Guid proposalId, List<RevisedEstimateCategoryDto> categories)
		{
			CostRevisionDto? costRevisionDto = null;
			var dbProposal = await ClientDbContext.Proposals
				.Include(p => p.ProposalLines)
				.FirstOrDefaultAsync(p => p.Id == proposalId);

			if (dbProposal == null) throw new Exception("Proposal not found!");

			var dbLineItems = dbProposal.ProposalLines.ToList();

			CostRevision? costRevision = null;
			foreach (var category in categories)
			{
				foreach (var lineItem in category.LineItems)
				{
					if (lineItem.CurrentRevisedValue != lineItem.Revised)
					{
						var proposalLineHistory = new ProposalLinesHistory();
						proposalLineHistory.HistoryId = Guid.NewGuid();
						proposalLineHistory.ProposalLineId = lineItem.ProposalLineId != null ? lineItem.ProposalLineId.Value : Guid.NewGuid();
						proposalLineHistory.ProposalId = proposalId;
						proposalLineHistory.ChangeType = "Updated";
						proposalLineHistory.ChangeDate = DateTime.UtcNow;
						proposalLineHistory.Amount = lineItem.Revised.Value;
						proposalLineHistory.EstimateCategoryId = lineItem.Id != null ? lineItem.Id.Value : Guid.NewGuid();
						proposalLineHistory.ParentEstimateCategoryId = lineItem.ParentId;
						ClientDbContext.ProposalLinesHistories.Add(proposalLineHistory);

						if(costRevision == null)
						{
							var latestRevisionNumber = await ClientDbContext.CostRevisions.MaxAsync(sr => (int?)sr.RevisionNumber) ?? 0;
							latestRevisionNumber += 1;

							costRevision = new CostRevision
							{
								Id = Guid.NewGuid(),
								RevisionNumber = latestRevisionNumber,
								RevisionDate = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
								ProjectId = dbProposal.QbclassId.Value,
								StatusId = (int)RevisionStatus.New
							};

							ClientDbContext.CostRevisions.Add(costRevision);
						}

						var revisionAmount = lineItem.Revised.Value - lineItem.CurrentRevisedValue.Value;
						costRevision.CostRevisionItems.Add(
							new CostRevisionItem
							{
								Id = Guid.NewGuid(),
								Amount = revisionAmount,
								CurrentAmount = lineItem.CurrentRevisedValue.Value,
								EstimateCategoryId = lineItem.EstimateCategoryId.Value,
								NewAmount = lineItem.Revised.Value
							});
					}
				}
			}
			await ClientDbContext.SaveChangesAsync();
			return mapper.Map<CostRevisionDto>(costRevision);
		}
		public async Task<List<EstimateCategoryDto>> GetAllEstimateCategoriesAsync(bool includeParents = true)
		{
			var estimateCategories = await ClientDbContext.EstimateCategories
				.OrderBy(c => c.ParentEstimateCategory.Sequence)
				.ThenBy(c => c.Sequence)
				.ToListAsync();

			if (!includeParents)
			{
				estimateCategories = estimateCategories.Where(e => e.ParentEstimateCategoryId != null).ToList();
			}

			return mapper.Map<List<EstimateCategoryDto>>(estimateCategories);
		}
		public async Task<bool> SaveRevisedEstimateMappingAsync(RevisedEstimateMappingPayload payload)
		{
			foreach(var mapping in payload.Mappings)
			{
				// add new estimate category, mapping and new proposal line
				if(mapping.EstimateCategoryId == Guid.Empty)
				{
					// add new estimate category
					var parentName = mapping.Parent;
					var estimateCategoryName = mapping.Name;

					var newEstimateCategory = await InsertNewEstimateCategoryAsync(estimateCategoryName, parentName);
					var estimateCategory = newEstimateCategory.EstimateCategory;
					var parentCategory = newEstimateCategory.ParentCategory;

					// add new mapping
					var estimateMapping = new EstimateMapping();
					estimateMapping.Id = Guid.NewGuid();
					estimateMapping.EstimateSubCategoryId = estimateCategory.Id;
					estimateMapping.AccountType = "Expenses";
					estimateMapping.QbaccountId = mapping.AccountId;
					estimateMapping.Created = DateTime.UtcNow;
					estimateMapping.CreatedBy = this.UserId.ToString();

					await ClientDbContext.EstimateMappings.AddAsync(estimateMapping);
					await ClientDbContext.SaveChangesAsync();

					// Only one proposal line per Item Name is allowed within a category
					var existingLineWithSameName = await ClientDbContext.ProposalLines
						.FirstOrDefaultAsync(pl =>
							pl.ProposalId == payload.ProposalId
							&& pl.ParentEstimateCategoryId == parentCategory.Id
							&& pl.Name.Trim().ToLower() == estimateCategoryName.Trim().ToLower());

					if (existingLineWithSameName == null)
					{
						var proposalLine = new ProposalLine();
						proposalLine.Id = Guid.NewGuid();
						proposalLine.ProposalId = payload.ProposalId;
						proposalLine.Amount = mapping.Amount;
						proposalLine.EstimateCategoryId = estimateCategory.Id;
						proposalLine.Name = estimateCategoryName;
						proposalLine.ParentEstimateCategoryId = parentCategory.Id;
						proposalLine.Sequence = estimateCategory.Sequence;
						ClientDbContext.ProposalLines.Add(proposalLine);
						await ClientDbContext.SaveChangesAsync();
					}
				}
				else
				{
					// Prefer an existing proposal line with the same Item Name under the same parent
					var selectedCategory = await ClientDbContext.EstimateCategories
						.FirstOrDefaultAsync(e => e.Id == mapping.EstimateCategoryId);

					var targetCategoryId = mapping.EstimateCategoryId;
					if (selectedCategory != null)
					{
						var existingLineWithSameName = await ClientDbContext.ProposalLines
							.FirstOrDefaultAsync(pl =>
								pl.ProposalId == payload.ProposalId
								&& pl.ParentEstimateCategoryId == selectedCategory.ParentEstimateCategoryId
								&& pl.Name.Trim().ToLower() == selectedCategory.Name.Trim().ToLower());

						if (existingLineWithSameName != null)
						{
							targetCategoryId = existingLineWithSameName.EstimateCategoryId;
						}
					}

					var existingMapping = await ClientDbContext.EstimateMappings
						.FirstOrDefaultAsync(m => m.QbaccountId == mapping.AccountId);

					if (existingMapping != null)
					{
						if (existingMapping.EstimateSubCategoryId != targetCategoryId)
						{
							existingMapping.EstimateSubCategoryId = targetCategoryId;
							existingMapping.Updated = DateTime.UtcNow;
							existingMapping.UpdatedBy = this.UserId.ToString();
						}
					}
					else
					{
						var estimateMapping = new EstimateMapping();
						estimateMapping.Id = Guid.NewGuid();
						estimateMapping.EstimateSubCategoryId = targetCategoryId;
						estimateMapping.AccountType = "Expenses";
						estimateMapping.QbaccountId = mapping.AccountId;
						estimateMapping.Created = DateTime.UtcNow;
						estimateMapping.CreatedBy = this.UserId.ToString();

						await ClientDbContext.EstimateMappings.AddAsync(estimateMapping);
					}

					await ClientDbContext.SaveChangesAsync();
				}
			}

			return true;
		}
		public async Task<bool> SaveEstimateCategoryAsync(EstimateCategoryPayload payload)
		{
			// add new estimate category
			var parentName = payload.ParentName;
			var estimateCategoryName = payload.Name;

			await InsertNewEstimateCategoryAsync(estimateCategoryName, parentName);
			return true;
		}
		public async Task<List<EstimateCategoryShortDetailsDto>> GetEstimateCategoriesByProjectId(Guid id)
		{
			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == id);
			if (proposal == null) throw new Exception("Proposal not found!");

			var proposalId = proposal.Id;
			var proposalLines = ClientDbContext.ProposalLines
				.Where(pl => pl.ProposalId == proposalId)
				.Select(pl => new
				{
					pl.EstimateCategoryId,
					pl.Name,
					Amount = ClientDbContext.ProposalLinesHistories
						.Where(plh => plh.ProposalId == proposalId && plh.EstimateCategoryId == pl.EstimateCategoryId)
						.OrderByDescending(plh => plh.ChangeDate)
						.Select(plh => (decimal?)plh.Amount)
						.FirstOrDefault() ?? pl.Amount
				})
				.OrderBy(pl => pl.Name)
				.ToList();

			var estimateCategories = proposalLines.Select(line => new EstimateCategoryShortDetailsDto
			{
				Id = line.EstimateCategoryId,
				Name = line.Name,
				Amount = line.Amount
			}).ToList();

			return estimateCategories;
		}
		public async Task<int> UpdateMinimumRequestedAmount(Guid projectId, decimal amount)
		{
			var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(c=> c.Id == projectId) ?? throw new Exception("Project not found.");
			qbClass.MinimumRequestedAmount = amount;
			qbClass.UpdatedBy = this.UserId.ToString();

			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == qbClass.Id);

			var cacheKey = $"EstimateToActual_{proposal.Id}";
			await cacheService.RemoveAsync(cacheKey);

			return await ClientDbContext.SaveChangesAsync();
		}

		#endregion

		#region Private Methods

		private async Task ProcessParentCategories(EstimateToActualDto estimateToActualDto, List<RevisedEstimateSpResult> dbRevisedEstimateData)
		{
			var parentCategories = dbRevisedEstimateData
				.Where(r => r.ParentEstimateCategory != null)
				.Select(r => new { r.ParentEstimateCategory, r.ParentSequence, r.ParentEstimateCategoryId })
				.Distinct()
				.OrderBy(r => r.ParentSequence)
				.ToList();

			var nonOverheadCategories = parentCategories.Where(c => c.ParentEstimateCategory.ToUpper() != "OVERHEAD").ToList();
			var overheadCategory = parentCategories.FirstOrDefault(c => c.ParentEstimateCategory.Trim().ToUpper() == "OVERHEAD");

			var dbParentCategories = new List<dynamic>();
			dbParentCategories.AddRange(nonOverheadCategories);
			if (overheadCategory != null)
			{
				dbParentCategories.Add(overheadCategory);
			}

			if (dbParentCategories.Any())
			{
				estimateToActualDto.EstimateCategories = new List<RevisedEstimateCategoryDto>();
				foreach (var parentCategory in dbParentCategories)
				{
					var category = new RevisedEstimateCategoryDto();
					category.Id = parentCategory.ParentEstimateCategoryId;
					category.Name = parentCategory.ParentEstimateCategory;
					category.Sequence = parentCategory.ParentSequence;
					category.LineItems = new List<RevisedEstimateCategoryLineDto>();

					var revisedEstimateLineItems = dbRevisedEstimateData
						.Where(r => r.ParentEstimateCategory == category.Name)
						.OrderBy(r => r.Sequence)
						.ToList();

					await ProcessLineItems(revisedEstimateLineItems, category);

					estimateToActualDto.EstimateCategories.Add(category);
				}
			}
		}
		private async Task ProcessSummary(EstimateToActualDto estimateToActualDto, List<RevisedEstimateSpResult> dbRevisedEstimateData, List<string> summaryItems)
		{
			
			var nonCategories = dbRevisedEstimateData
				.Where(r => summaryItems.Contains(r.Name))
				.OrderBy(r => r.Sequence)
				.ToList();

			estimateToActualDto.Summary = new Summary();
			estimateToActualDto.Summary.TotalCostToDate = nonCategories.FirstOrDefault(r => r.Name == "TOTAL COST TO DATE")?.CostToDate ?? 0;
			estimateToActualDto.Summary.OwnerDeposits = nonCategories.FirstOrDefault(r => r.Name == "OWNER DEPOSITS")?.CostToDate ?? 0;
			estimateToActualDto.Summary.JobBalance = nonCategories.FirstOrDefault(r => r.Name == "JOB BALANCE")?.CostToDate ?? 0;
			estimateToActualDto.Summary.MinimumRequestedAmount = nonCategories.FirstOrDefault(r => r.Name == "MINIMUM REQUESTED AMOUNT")?.CostToDate ?? 0;

			var unMappedTransactions = dbRevisedEstimateData
				.Where(r => r.AccountId != null)
				.ToList();

			if (unMappedTransactions.Any())
			{
				estimateToActualDto.Summary.TotalCostToDate += unMappedTransactions.Sum(a => a.CostToDate.Value);
			}

		}
		private async Task ProcessUnmappedTransactions(EstimateToActualDto estimateToActualDto, List<RevisedEstimateSpResult> dbRevisedEstimateData)
		{
			var unMappedTransactions = dbRevisedEstimateData
				.Where(r => r.AccountId != null)
				.ToList();

			if (unMappedTransactions.Any())
			{
				estimateToActualDto.UnmappedTransactions = new List<UmmappedTransactions>();
				foreach (var transaction in unMappedTransactions)
				{
					estimateToActualDto.UnmappedTransactions.Add(new UmmappedTransactions
					{
						AccountId = transaction.AccountId.Value,
						CostToDate = transaction.CostToDate,
						Name = transaction.Name
					});
				}

				//insert a dummy category for unmapped transactions
				var overheadCategory = estimateToActualDto.EstimateCategories
					.FirstOrDefault(c => c.Name.Trim().ToUpper() == "OVERHEAD");

				if (overheadCategory != null)
				{
					// For UI Purposes, hard-code this id for now since it has no corresponding record
					var dummyEstimateCategoryId = new Guid("821DBDD6-A6FE-4ED3-8F98-2AA63DABFEBA"); 
					var lineItem = new RevisedEstimateCategoryLineDto();
					lineItem.Id = dummyEstimateCategoryId;
					lineItem.ProposalLineId = dummyEstimateCategoryId;
					lineItem.EstimateCategoryId = dummyEstimateCategoryId;
					lineItem.ParentId = overheadCategory.Id;
					lineItem.CurrentRevisedValue = 0;
					lineItem.Revised = 0;
					lineItem.Balance = 0;
					lineItem.CostToDate = unMappedTransactions.Sum(a => a.CostToDate.Value);
					lineItem.Percentage = 0;
					lineItem.Sequence = overheadCategory.LineItems.Count() + 1;
					lineItem.Original = 0;
					lineItem.Name = "Unmapped Transactions";
					overheadCategory.LineItems.Add(lineItem);
				}

			}
		}
		private async Task<(EstimateCategory EstimateCategory, EstimateCategory ParentCategory)> InsertNewEstimateCategoryAsync(string name, string parentName)
		{
			var parentCategory = await ClientDbContext.EstimateCategories
				.FirstOrDefaultAsync(e => e.Name.Trim().ToLower() == parentName.Trim().ToLower());

			if (parentCategory == null)
			{
				var maxSequence = await ClientDbContext.EstimateCategories.Where(e => e.ParentEstimateCategoryId == null).MaxAsync(e => e.Sequence);
				parentCategory = new EstimateCategory();
				parentCategory.Name = parentName;
				parentCategory.Sequence = maxSequence + 1;
				parentCategory.ParentEstimateCategoryId = null;
				parentCategory.CreatedBy = this.UserId.ToString();
				parentCategory.Created = DateTime.UtcNow;

				await ClientDbContext.EstimateCategories.AddAsync(parentCategory);
				await ClientDbContext.SaveChangesAsync();

			}
			var childCategories = ClientDbContext.EstimateCategories.Where(e => e.ParentEstimateCategoryId == parentCategory.Id);
			var existingChild = await childCategories
				.FirstOrDefaultAsync(e => e.Name.Trim().ToLower() == name.Trim().ToLower());

			if (existingChild != null)
			{
				return (existingChild, parentCategory);
			}

			var maxChildSequence = childCategories.Any() ? await childCategories.MaxAsync(e => e.Sequence) : 1;
			var estimateCategory = new EstimateCategory();
			estimateCategory.Id = Guid.NewGuid();
			estimateCategory.ParentEstimateCategoryId = parentCategory.Id;
			estimateCategory.Name = name;
			estimateCategory.Sequence = maxChildSequence + 1;
			estimateCategory.CreatedBy = this.UserId.ToString();
			estimateCategory.Created = DateTime.UtcNow;

			await ClientDbContext.EstimateCategories.AddAsync(estimateCategory);
			await ClientDbContext.SaveChangesAsync();

			return (estimateCategory, parentCategory);
		}
		private async Task ProcessLineItems(List<RevisedEstimateSpResult> lineItems, RevisedEstimateCategoryDto category)
		{
			await Task.Run(() =>
			{
				var uniqueLineItems = DeduplicateRevisedEstimateLineItemsByName(lineItems);
				foreach (var estimateLineItem in uniqueLineItems)
				{
					var lineItem = new RevisedEstimateCategoryLineDto();
					lineItem.Id = estimateLineItem.EstimateCategoryId;
					lineItem.ProposalLineId = estimateLineItem.EstimateCategoryId;
					lineItem.EstimateCategoryId = estimateLineItem.EstimateCategoryId;
					lineItem.ParentId = estimateLineItem.ParentEstimateCategoryId;
					lineItem.CurrentRevisedValue = estimateLineItem.RevisedAmount;
					lineItem.Revised = estimateLineItem.RevisedAmount;
					lineItem.Balance = estimateLineItem.Balance;
					lineItem.CostToDate = estimateLineItem.CostToDate;
					lineItem.HasEstimateMapping = estimateLineItem.HasEstimateMapping ?? true;
					lineItem.Percentage = estimateLineItem.Percentage;
					lineItem.Sequence = estimateLineItem.Sequence;
					lineItem.Original = estimateLineItem.OriginalAmount;
					lineItem.Name = estimateLineItem.Name;
					category.LineItems.Add(lineItem);
				}
			});
		}

		/// <summary>
		/// Keeps a single ETA line item per Item Name within a category (case-insensitive, trimmed).
		/// Prefers the highest revised/original amount, then lowest sequence. Cost-to-date is summed across duplicates.
		/// </summary>
		private static List<RevisedEstimateSpResult> DeduplicateRevisedEstimateLineItemsByName(IEnumerable<RevisedEstimateSpResult> lineItems)
		{
			if (lineItems == null)
			{
				return new List<RevisedEstimateSpResult>();
			}

			var uniqueItems = lineItems
				.Where(item => !string.IsNullOrWhiteSpace(item.Name))
				.GroupBy(item => item.Name.Trim().ToLowerInvariant())
				.Select(group =>
				{
					var preferred = group
						.OrderByDescending(item => item.RevisedAmount ?? 0)
						.ThenByDescending(item => item.OriginalAmount ?? 0)
						.ThenBy(item => item.Sequence ?? int.MaxValue)
						.First();

					var hasEstimateMapping = preferred.HasEstimateMapping ?? true;
					var costToDate = hasEstimateMapping
						? group.Sum(item => item.CostToDate ?? 0)
						: (decimal?)null;
					var revised = preferred.RevisedAmount ?? 0;

					preferred.HasEstimateMapping = hasEstimateMapping;
					preferred.CostToDate = costToDate;
					var costForBalance = costToDate ?? 0;
					preferred.Balance = revised - costForBalance;
					preferred.Percentage = revised == 0
						? null
						: Math.Round(costForBalance == revised ? 100 : (costForBalance / revised) * 100);

					return preferred;
				})
				.OrderBy(item => item.Sequence ?? int.MaxValue)
				.ToList();

			for (var i = 0; i < uniqueItems.Count; i++)
			{
				uniqueItems[i].Sequence = i + 1;
			}

			return uniqueItems;
		}
		private async Task<List<RevisedEstimateSpResult>> GetRevisedEstimateSpResult(Guid proposalId)
		{
			return await this.ClientDbContext.RevisedEstimateSpResult
			.FromSqlRaw($"EXEC spCDGetRevisedEstimate '{proposalId}'")
			.ToListAsync();
		}
		private (string Start, string End) GetDateRange(DateTime? minimumStartDate = null)
		{
			var dateFormat = "MM/dd/yyyy";
			var minStartDate = minimumStartDate != null ? minimumStartDate.Value.ToString(dateFormat) : configuration["Defaults:MinimumStartDate"];
			var endDate = DateTime.UtcNow.ToString(dateFormat);
			return (Start: minStartDate, End: endDate);
		}
		private async Task<bool> SynchTransactionsToEstimates(Guid proposalId)
		{
			await this.ClientDbContext.Database.ExecuteSqlRawAsync("EXEC spSynchTransactionsToEstimates @ProposalID = {0}", proposalId);
			return true;
		}
		private async Task<bool> EnsureUniqueProposalLinesAsync(Guid proposalId)
		{
			await this.ClientDbContext.Database.ExecuteSqlRawAsync("EXEC spEnsureUniqueProposalLines @ProposalID = {0}", proposalId);
			return true;
		}
		#endregion

	}
}
