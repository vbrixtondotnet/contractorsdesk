using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IReportsService : IBaseService
	{
		Task<List<JobsSummaryReportDto>> GetActiveJobsSummaryAsync();
		Task<List<JobsSummaryReportDto>> GetPendingJobsSummaryAsync();
		Task<List<ActiveConstructionJobSpResult>> GetActiveConstructionJobReportAsync(DateOnly start, DateOnly end, string filter);
		Task<List<ClassTransactionsReportSpResult>> GetClassTransactionsReportAsync(DateOnly start, DateOnly end, string className, string filter);
		Task<List<TransactionsReportSpResult>> GetTransactionsReportAsync(DateOnly start, DateOnly end, string className, string filter);
	}
}
