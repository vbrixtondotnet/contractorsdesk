using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ContractorsDesk.Services
{
    public class ReportsService : BaseService, IReportsService
	{
		private readonly IProjectsService projectsService;
		public ReportsService(
			ClientDbContext clientDataDbContext,
			MasterDbContext masterDbContext,
			IProjectsService projectsService,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDataDbContext, masterDbContext, configuration)
		{
			this.projectsService = projectsService;
		}
		public async Task<List<JobsSummaryReportDto>> GetActiveJobsSummaryAsync()
		{
			var activeJobs = await projectsService.GetActiveProjectsAsync();
			return mapper.Map<List<JobsSummaryReportDto>>(activeJobs);
		}

		public async Task<List<JobsSummaryReportDto>> GetPendingJobsSummaryAsync()
		{
			var pendingJobs = await projectsService.GetPendingProjectsAsync();
			return mapper.Map<List<JobsSummaryReportDto>>(pendingJobs);
		}
		public async Task<List<ActiveConstructionJobSpResult>> GetActiveConstructionJobReportAsync(DateOnly start, DateOnly end, string filter)
		{
			var startDate = start.ToString("yyyy-MM-dd");
			var endDate = end.ToString("yyyy-MM-dd");

			return await this.ClientDbContext.ActiveConstructionJobsSpResult
				.FromSqlRaw($"EXEC spActiveConstructionJobs '{startDate}','{endDate}','{filter}'")
				.ToListAsync();
		}

		public async Task<List<ClassTransactionsReportSpResult>> GetClassTransactionsReportAsync(DateOnly start, DateOnly end, string className, string filter)
		{
			var startDate = start.ToString("yyyy-MM-dd");
			var endDate = end.ToString("yyyy-MM-dd");

			return await this.ClientDbContext.ClassTransactionsReportSpResult
				.FromSqlRaw($"EXEC spClassTransactionsReport '{startDate}','{endDate}','{className}','{filter}'")
				.ToListAsync();
		}

		public async Task<List<TransactionsReportSpResult>> GetTransactionsReportAsync(DateOnly start, DateOnly end, string className, string filter)
		{
			var startDate = start.ToString("yyyy-MM-dd");
			var endDate = end.ToString("yyyy-MM-dd");

			return await this.ClientDbContext.TransactionsReportSpResult
				.FromSqlRaw($"EXEC spTransactionsReport '{startDate}','{endDate}','{className}','{filter}'")
				.ToListAsync();
		}
	}
}
