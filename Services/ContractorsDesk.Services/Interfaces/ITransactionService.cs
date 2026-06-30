using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface ITransactionService : IBaseService
	{
		Task<TransactionDetailReportSpResult?> GetLatestOwnerDeposit(Guid proposalId);
		Task<List<TransactionDetailsDto>> GetTransactionDetailsAsync(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId, DateOnly? startDate = null, DateOnly? endDate = null);
		Task<decimal> GetTotalCost(Guid projectId);
	}
}
