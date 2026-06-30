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
using ContractorsDesk.Core.Utilities;

namespace ContractorsDesk.Services
{
	public class ProjectJournalService : BaseService, IProjectJournalService
	{
		private readonly ICacheService cacheService;
		#region Public Methods
		public ProjectJournalService(
			ICacheService cacheService,
			ClientDbContext clientDbContext, 
			IMapper mapper)
			: base(mapper, clientDbContext) 
		{
			this.cacheService = cacheService;
		}
		public async Task<ProjectJournalDto> GetProjectJournalByProjectIdAsync(Guid projectId)
		{
			var projectJournal = await ClientDbContext.ProjectJournals
			.Where(x => x.ProjectId == projectId)
			.OrderByDescending(x => x.DateCreated)
			.AsNoTracking()
			.ToListAsync();

			var firstJournal = projectJournal.FirstOrDefault();
			var retval = firstJournal == null ? null : mapper.Map<ProjectJournalDto>(firstJournal);

			if (retval != null)
			{
				for (int i = 1; i < projectJournal.Count; i++)
				{
					var currentElement = projectJournal[i];
					retval.Journal += currentElement.Journal;
				}
			}

			return retval;
		}

		public async Task<ProjectJournalDto> GetByIdAsync(Guid id)
		{
			var projectJournal = ClientDbContext.ProjectJournals
				.Include(x => x.Project)
				.Where(x => x.Id == id)
				.AsNoTracking()
				.FirstOrDefault();

			return mapper.Map<ProjectJournalDto>(projectJournal);
		}

		public async Task<ProjectJournalDto> SaveAsync(Guid projectId, ProjectJournalPayload projectJournal, bool prepend = false)
		{

			var dbProjectJournal = new ProjectJournal();
			dbProjectJournal.Id = Guid.NewGuid();
			dbProjectJournal.ProjectId = projectId;
			dbProjectJournal.Journal = projectJournal.Journal;
			dbProjectJournal.DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();

			ClientDbContext.ProjectJournals.Add(dbProjectJournal);

			await ClientDbContext.SaveChangesAsync();
			return mapper.Map<ProjectJournalDto>(dbProjectJournal);
        }

		public async Task<DateTime> GetLatestJournalDateAsync(Guid? projectId)
		{
			var query = ClientDbContext.ProjectJournals.OrderByDescending(x => x.DateCreated).AsNoTracking();

			if (projectId != null)
			{
				query = query.Where(x => x.ProjectId == projectId);
			}

			var latestJournal = await query.FirstOrDefaultAsync();

			return latestJournal?.DateCreated ?? DateTime.MinValue;
		}

		#endregion

		#region Private Methods

		#endregion
	}
}
