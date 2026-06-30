using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using Microsoft.Extensions.Configuration;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class TransactionService : BaseService, ITransactionService
	{
		private readonly IEstimateService estimateService;
		public TransactionService(
			ClientDbContext clientDataDbContext,
			MasterDbContext masterDbContext,
			IEstimateService estimateService,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDataDbContext, masterDbContext, configuration)
		{
			this.estimateService = estimateService;
		}

		public async Task<List<TransactionDetailsDto>> GetTransactionDetailsAsync(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			var retval = new List<TransactionDetailsDto>();
			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == proposalId);
			if (proposal == null) throw new Exception("Proposal not found.");

			var transactionDetails = await GetTransactionDetailsFromSpAsync(proposalId, estimateCategoryId, parentEstimateCategoryId, startDate, endDate);

			var categories = transactionDetails
				.Select(td => new { td.EstimateCategoryId, Name = td.EstimateCategory, Sequence = td.CategorySequence })
				.Distinct()
				.ToList();

			foreach (var category in categories)
			{
				var transactionCategory = new TransactionDetailsDto { 
					Category = category.Name, 
					Sequence = category.Sequence.Value, 
					LineItems = new List<TransactionDetailsLineItemDto>()
				};

				var lineItems = transactionDetails
					.Where(td=> td.EstimateCategoryId == category.EstimateCategoryId)
					.Select(li=> new { Name = li.EstimateSubCategory, Sequence = li.ItemSequence, li.EstimateCategoryId, li.RevisedEstimate})
					.Distinct()
					.ToList();

				foreach (var lineItem in lineItems)
				{
					var categoryLineItem = new TransactionDetailsLineItemDto { Name = lineItem.Name, Sequence = lineItem.Sequence.Value, Transactions = new List<TransactionsDto>() };
					var transactionLineItems = transactionDetails.Where(td=> td.EstimateSubCategory == lineItem.Name).ToList();

                    foreach (var item in transactionLineItems)
                    {
						var transaction = new TransactionsDto { Amount = item.Amount.Value, Date = item.Date.Value, Payee = item.Payee, Memo = item.Memo, Num = item.Num, Type = item.Type };
						categoryLineItem.Transactions.Add(transaction);
					}
					categoryLineItem.RevisedEstimate = lineItem.RevisedEstimate;
					categoryLineItem.Transactions = categoryLineItem.Transactions.OrderBy(td=>td.Date).ToList();
					transactionCategory.LineItems.Add(categoryLineItem);
				}
				retval.Add(transactionCategory);
			}

			return retval;
		}

		public async Task<TransactionDetailReportSpResult?> GetLatestOwnerDeposit(Guid proposalId)
		{
			var retval = new List<TransactionDetailsDto>();
			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.Id == proposalId);
			if (proposal == null) throw new Exception("Proposal not found.");

			var ownerDepositsId = new Guid("36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38");

			var transactionDetails = await GetTransactionDetailsFromSpAsync(proposalId, null, ownerDepositsId);

			return transactionDetails.OrderByDescending(td => td.Date).FirstOrDefault();
		}

		public async Task<decimal> GetTotalCost(Guid projectId)
		{
			var totalCostFromSpResult = await GetTotalCostFromSpAsync(projectId);
			return totalCostFromSpResult.TotalCost;
		}

		private async Task<List<TransactionDetailReportSpResult>> GetTransactionDetailsFromSpAsync(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId, DateOnly? startDate = null, DateOnly? endDate = null)
		{
			var query = parentEstimateCategoryId != null
				? $"EXEC spCDGetTransactionDetails '{proposalId}', null, '{parentEstimateCategoryId}', null, null"
				: $"EXEC spCDGetTransactionDetails '{proposalId}', '{estimateCategoryId}', null, null, null";

			if (startDate != null && endDate != null)
			{
				var start = startDate?.ToString("MM/dd/yyyy");
				var end = endDate?.ToString("MM/dd/yyyy");
				query = parentEstimateCategoryId != null
				? $"EXEC spCDGetTransactionDetails '{proposalId}', null, '{parentEstimateCategoryId}', '{start}', '{end}'"
				: $"EXEC spCDGetTransactionDetails '{proposalId}', '{estimateCategoryId}', null, '{start}', '{end}'";
			}

			return await this.ClientDbContext.TransactionDetailReportSpResult
				.FromSqlRaw(query)
				.ToListAsync();
		}

		private async Task<TotalCostSpResult?> GetTotalCostFromSpAsync(Guid projectId)
		{
			var retval = await this.ClientDbContext.TotalCostSpResult
				.FromSqlRaw($"EXEC spGetTotalCost '{projectId}'")
				.ToListAsync();

			return retval.FirstOrDefault();
		}
	}
}
