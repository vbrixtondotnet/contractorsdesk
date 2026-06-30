using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.ApiPayloadModels;
using Pipelines.Sockets.Unofficial.Arenas;

namespace ContractorsDesk.Services
{
	public class DashboardService : BaseService, IDashboardService
	{
		private readonly ICacheService cacheService;

		public DashboardService(
			ClientDbContext clientDbContext, 
			IMapper mapper)
			: base(mapper, clientDbContext) 
		{
			this.cacheService = cacheService;
		}

		public async Task<DashboardStatsDto> GetDashboardStatsAsync(bool canManageAllProjects)
		{
			var dashboardStats = new DashboardStatsDto();

			// Current Projects
			var activeJobs = await ClientDbContext.VwSupervisorAndClientActiveJobs
				.OrderBy(v => v.Name)
				.ToListAsync();

			var supervisorJobs = activeJobs.Select(ac => new { ac.SupervisorId, ac.SupervisorFirstName, ac.SupervisorLastName, JobId = ac.Id });

			if (!canManageAllProjects)
			{
				activeJobs = activeJobs
					.Where(ac => ac.SupervisorId == this.UserId)
					.ToList();
			}

			var projectStats = new ProjectStats();

			activeJobs = activeJobs.DistinctBy(ac => ac.Id).ToList();

			projectStats.ArchivedCount = activeJobs.Count(a => a.IsArchived);
			projectStats.ActiveCount = activeJobs.Count(a => !a.IsArchived);
			projectStats.CompletedCount = activeJobs.Count(a => a.IsCompleted);
			dashboardStats.ProjectStats = projectStats;

			var projectBalances = new ProjectBalances();

			activeJobs = activeJobs.Where(a => !a.IsArchived).ToList();
			projectBalances.NegativeBalanceCount = activeJobs.Count(a => a.JobBalance < 0);
			projectBalances.BelowThresholdCount = activeJobs.Count(a => (a.JobBalance < a.Threshold) && a.JobBalance >= 0);
			projectBalances.AboveThresholdCount = activeJobs.Count(a => a.JobBalance >= 0 && (a.JobBalance >= a.Threshold));
			dashboardStats.ProjectBalances = projectBalances;

			var dbClients = ClientDbContext.Clients.OrderByDescending(c=> c.DateCreated).ToList();

			var recentClients = new List<RecentClient>();

			var dbRecentClients = dbClients.Where(c => !string.IsNullOrEmpty(c.Name)).DistinctBy(c=> c.Name).Take(8).ToList();

			foreach(var dbRecentClient in dbRecentClients)
			{
				recentClients.Add(new RecentClient { Name = dbRecentClient.Name });
			}

			dashboardStats.RecentClients = recentClients;
			dashboardStats.TotalClients = dbClients.Count();

			return dashboardStats;
		}
	}
}
