using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.Core.Dto.@base;
using System.Threading.Tasks;
using System.Linq;
using ContractorsDesk.Core.ApiPayloadModels;
using DocuSign.eSign.Model;
using MimeKit.Utils;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Core.Enums;

namespace ContractorsDesk.Services
{
	public class ScheduleService : BaseService, IScheduleService
	{
		private readonly IProjectsService projectsService;
		private readonly ICacheService cacheService;
		public ScheduleService(
			IProjectsService projectsService,
			ICacheService cacheService,
			ClientDbContext clientDataDbContext, 
			IMapper mapper)
			: base(mapper, clientDataDbContext) 
		{
			this.projectsService = projectsService;
			this.cacheService = cacheService;
		}

		#region Public Methods
		public async Task<ProjectScheduleViewDto> GetProjectScheduleAsync(Guid projectId)
		{		
			var retval = new ProjectScheduleViewDto();
			var projectDetails = await projectsService.GetProjectShortDetailsAsync(projectId);
			var projectSchedule = await ClientDbContext.ProjectSchedules.FirstOrDefaultAsync(ps => ps.ProjectId == projectId);

			if (projectSchedule == null)
			{
				retval.Status = "Draft";
				await GetMappedConstructionTasks(projectId, retval);
			}
			else
			{
				var constructionTasks = await ClientDbContext.ProjectScheduleTasks
					.Where(ps => ps.ProjectScheduleId == projectSchedule.Id)
					.OrderBy(ct => ct.Sequence)
					.ToListAsync();

				var delays = await ClientDbContext.ProjectScheduleDelays
					.Where(d => d.ProjectScheduleId == projectSchedule.Id)
					.Include(d => d.Task)
					.ToListAsync();
				var endDateStr = projectDetails?.EndDate;

				retval.Status = projectSchedule.Status;
				retval.StartDate = projectSchedule.StartDate.ToDateTime(TimeOnly.MinValue);
				retval.Tasks = mapper.Map<List<ProjectScheduleTaskDto>>(constructionTasks);
				retval.Delays = mapper.Map<List<ProjectScheduleDelayDto>>(delays);

				if (!string.IsNullOrEmpty(endDateStr))
				{
					retval.EstimatedCompletionDate = DateOnly.Parse(endDateStr).ToDateTime(TimeOnly.MinValue);
				}
			}

			retval.ProjectDetails = projectDetails;
			return retval;
		}
		public async Task<ScheduleRevisionDto> GetScheduleRevision(Guid scheduleRevisionId)
		{
			var scheduleRevision = await ClientDbContext.ScheduleRevisions
				.AsNoTracking()
				.Include(sr => sr.Project)
					.ThenInclude(p=> p.Proposals)
				.Include(sr => sr.Project)
					.ThenInclude(p => p.ProjectSchedules)
					.ThenInclude(sr => sr.ProjectScheduleTasks)
				.Include(sr=> sr.ScheduleRevisionItems)
				.FirstOrDefaultAsync(sr => sr.Id == scheduleRevisionId) ?? throw new Exception("Schedule Revision not found!");

			var retval = mapper.Map<ScheduleRevisionDto>(scheduleRevision);
			var proposal = scheduleRevision.Project?.Proposals
				.FirstOrDefault();

			var schedule = scheduleRevision.Project?.ProjectSchedules
				.FirstOrDefault();

			var tasks = schedule.ProjectScheduleTasks;

			var qbCustomer = await ClientDbContext.Clients
				.FirstOrDefaultAsync(c => c.Id == proposal.ClientId);

			retval.ProjectName = scheduleRevision.Project?.Name ?? string.Empty;
			retval.ProjectAddress = scheduleRevision.Project?.Address ?? string.Empty;
			retval.ClientName = qbCustomer.Name;
			retval.StartDate = schedule.StartDate;

			foreach ( var item in retval.ScheduleRevisionItems)
			{
				var constructionTask = tasks.FirstOrDefault(t => t.ConstructionTaskId == item.ConstructionTaskId);
				item.ConstructionTask = constructionTask?.Name ?? string.Empty;
			}

			return retval;
		}
		public async Task<ProjectScheduleDto> GetProjectScheduleConstructionTasks(Guid projectId)
		{
			var retval = new ProjectScheduleDto();
			var qbClass = await ClientDbContext.Qbclasses.FirstOrDefaultAsync(ps => ps.Id == projectId);
			var projectSchedule = await ClientDbContext.ProjectSchedules.FirstOrDefaultAsync(ps => ps.ProjectId == projectId);

			retval.ProjectId = projectId;
			retval.ProjectName = qbClass?.Name;

			if (projectSchedule == null)
			{
				retval.Tasks = new List<ProjectScheduleTaskDto>();
			}
			else {
				var constructionTasks = await ClientDbContext.ProjectScheduleTasks
					.Where(ps => ps.ProjectScheduleId == projectSchedule.Id)
					.OrderBy(ct => ct.Sequence)
					.ToListAsync();

				retval.Tasks = mapper.Map<List<ProjectScheduleTaskDto>>(constructionTasks);
			}

			return retval;
		}
		public async Task<ProjectScheduleViewDto> SaveProjectScheduleAsync(Guid projectId, ProjectSchedulePayload payload)
		{
			var dbProjectSchedule = await ClientDbContext.ProjectSchedules
				.FirstOrDefaultAsync(s => s.ProjectId == projectId);

            if (dbProjectSchedule == null)
            {
				dbProjectSchedule = new ProjectSchedule();
				mapper.Map(payload, dbProjectSchedule);
				dbProjectSchedule.ProjectId = projectId;
				dbProjectSchedule.Id = Guid.NewGuid();
				ClientDbContext.ProjectSchedules.Add(dbProjectSchedule);
			}
			else
			{
				mapper.Map(payload, dbProjectSchedule);
				ClientDbContext.Entry(dbProjectSchedule).State = EntityState.Modified;
			}

			await UpdateDelays(payload, dbProjectSchedule.Id);
			await UpdateTasks(payload, dbProjectSchedule.Id);
			var scheduleRevision = await UpdateScheduleRevisions(payload.ScheduleRevisions, projectId, dbProjectSchedule.Id);

			await ClientDbContext.SaveChangesAsync();

			var retval = await GetProjectScheduleAsync(projectId);
			retval.ScheduleRevision = scheduleRevision;
			return retval;
		}
		public async Task<ProjectScheduleDelayDto> AddProjectDelay(Guid projectId, ProjectScheduleDelayDto projectScheduleDelayDto)
		{
			var cacheKey = $"ProjectSchedule_{projectId}";
			var projectSchedule = await ClientDbContext.ProjectSchedules.FirstOrDefaultAsync(pt => pt.ProjectId == projectId);

			if (projectSchedule == null) throw new Exception("Project Schedule not found.");

            var projectScheduleDelay = new ProjectScheduleDelay();
			mapper.Map(projectScheduleDelayDto, projectScheduleDelay);
			projectScheduleDelay.Id = Guid.NewGuid();
			projectScheduleDelay.ProjectScheduleId= projectSchedule.Id;
			ClientDbContext.ProjectScheduleDelays.Add(projectScheduleDelay);
			await ClientDbContext.SaveChangesAsync();


			await cacheService.RemoveAsync(cacheKey);
			return mapper.Map<ProjectScheduleDelayDto>(projectScheduleDelay);
		}
		public async Task<List<ProposalLineItemDto>> GetUnmappedProposalLines(Guid projectId)
		{
			var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == projectId);
			if (proposal == null)
			{
				return Enumerable.Empty<ProposalLineItemDto>().ToList();
			}

			var unscheduledProposalLines = await ClientDbContext.ProposalLines
				.Where(pl => pl.ProposalId == proposal.Id &&
							 !ClientDbContext.ScheduleTaskMappings
								 .Any(sm => sm.EstimateCategoryId == pl.EstimateCategoryId))
				.ToListAsync();

			return unscheduledProposalLines.Select(mapper.Map<ProposalLineItemDto>).ToList();
		}
		#endregion

		#region Private Methods
		private async Task UpdateTasks(ProjectSchedulePayload projectSchedule, Guid projectScheduleId)
		{
			//clientDbContext.ProjectScheduleTasks.RemoveRange(tasks);
			var currentTasks = await ClientDbContext.ProjectScheduleTasks.Where(t => t.ProjectScheduleId == projectScheduleId)
				.ToListAsync();

			var currentTaskIds = currentTasks.Select(t => t.Id).ToHashSet();
			var updatedTasksIds = projectSchedule.Tasks.Select(t => t.Id).ToHashSet();

			var tasksForUpdate = projectSchedule.Tasks
				.Where(t => t.Id != null && currentTaskIds.Contains(t.Id.Value))
				.ToList();

			foreach (var taskDto in tasksForUpdate) {
				var task = currentTasks.FirstOrDefault(t => t.Id == taskDto.Id);

				task.Duration = taskDto.Duration;
				task.StartDate = DateOnly.FromDateTime(taskDto.StartDate.Value);
				task.EndDate = DateOnly.FromDateTime(taskDto.EndDate.Value);
				task.Sequence = taskDto.Sequence;
				task.Lag1 = taskDto.Lag1;
				task.Pred1 = taskDto.Pred1;
			}

			var deletedTasks = currentTasks
				.Where(t => !updatedTasksIds.Contains(t.Id))
				.ToList();

			foreach(var deletedTask in deletedTasks)
			{
				var delay = await ClientDbContext.ProjectScheduleDelays.FirstOrDefaultAsync(t => t.TaskId == deletedTask.Id);

				if (delay != null)
				{
					ClientDbContext.ProjectScheduleDelays.Remove(delay);
				}

				//var scheduleRevision = await clientDbContext.ScheduleRevisions.FirstOrDefaultAsync(s=> s.ConstructionTaskId == deletedTask.ConstructionTaskId);
    //            if (scheduleRevision != null)
    //            {
				//	clientDbContext.ScheduleRevisions.Remove(scheduleRevision);
				//}

                ClientDbContext.ProjectScheduleTasks.Remove(deletedTask);
			}
			
			var newTasks = projectSchedule.Tasks
				.Where(t => t.Id == null || !currentTaskIds.Contains(t.Id.Value))
				.ToList();

			foreach (var taskDto in newTasks)
			{
				var dbProjectScheduleTask = mapper.Map<ProjectScheduleTask>(taskDto);
				dbProjectScheduleTask.Id = Guid.NewGuid();
				dbProjectScheduleTask.ProjectScheduleId = projectScheduleId;
				ClientDbContext.ProjectScheduleTasks.Add(dbProjectScheduleTask);
			}
		}
		private async Task UpdateDelays(ProjectSchedulePayload projectSchedule, Guid projectScheduleId)
		{		
			var newDelays = projectSchedule.Delays
				.Where(dto => dto.New)
				.ToList();

			foreach (var delayDto in newDelays)
			{
				var dbProjectScheduleDelay = mapper.Map<ProjectScheduleDelay>(delayDto);
				dbProjectScheduleDelay.Id = Guid.NewGuid();
				dbProjectScheduleDelay.ProjectScheduleId = projectScheduleId;
				ClientDbContext.ProjectScheduleDelays.Add(dbProjectScheduleDelay);
			}

			var updatedDelays = projectSchedule.UpdatedDelays;
			foreach (var updatedDelay in updatedDelays)
			{
				var delay = await ClientDbContext.ProjectScheduleDelays.FirstOrDefaultAsync(d=> d.Id == updatedDelay.Id);
				if (delay != null)
				{
					delay.Days = updatedDelay.Days;
					delay.Reason = updatedDelay.Reason;
					delay.Description = updatedDelay.Description;
				}
			}

		}
		private async Task<ScheduleRevisionDto?> UpdateScheduleRevisions(List<ScheduleRevisionPayload> revisions, Guid projectId, Guid projectScheduleId)
		{
			ScheduleRevisionDto? retval = null;

			if (revisions.Count > 0)
			{
				var projectSchedule = await ClientDbContext.ProjectSchedules
				.AsNoTracking()
				.Where(ps => ps.Id == projectScheduleId)
				.Include(ps => ps.ProjectScheduleTasks)
				.FirstOrDefaultAsync();

				if (projectSchedule != null)
				{
					var latestRevisionNumber = await ClientDbContext.ScheduleRevisions
						.MaxAsync(sr => (int?)sr.RevisionNumber) ?? 0;

					latestRevisionNumber += 1;

					var dbScheduleRevision = new ScheduleRevision();
					dbScheduleRevision.Id = Guid.NewGuid();
					dbScheduleRevision.ProjectId = projectId;
					dbScheduleRevision.RevisionNumber = latestRevisionNumber;
					dbScheduleRevision.RevisionDate = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
					dbScheduleRevision.StatusId = (int)RevisionStatus.New;

					var projectScheduleItems = projectSchedule.ProjectScheduleTasks.ToList();

					foreach (var revision in revisions)
					{
						var projectScheduleItem = projectScheduleItems.FirstOrDefault(ps => ps.ConstructionTaskId == revision.ConstructionTaskId);

						if (projectScheduleItem != null)
						{
							var dbScheduleRevisionItem = mapper.Map<ScheduleRevisionItem>(revision);
							dbScheduleRevisionItem.Id = Guid.NewGuid();
							dbScheduleRevisionItem.OldDuration = projectScheduleItem.Duration;
							dbScheduleRevisionItem.OldStartDate = projectScheduleItem.StartDate;
							dbScheduleRevisionItem.OldEndDate = projectScheduleItem.EndDate;
							dbScheduleRevision.ScheduleRevisionItems.Add(dbScheduleRevisionItem);
						}
					}

					ClientDbContext.ScheduleRevisions.Add(dbScheduleRevision);

					retval = mapper.Map<ScheduleRevisionDto>(dbScheduleRevision);
				}
			}
			
			return retval;
		}

		private async Task GetMappedConstructionTasks(Guid projectId, ProjectScheduleViewDto projectScheduleDTO)
		{

			var unmappedProposalLines = await GetUnmappedProposalLines(projectId);

			if (unmappedProposalLines.Count == 0)
			{
				var proposal = await ClientDbContext.Proposals.FirstOrDefaultAsync(p => p.QbclassId == projectId);
				if (proposal == null)
				{
					return;
				}

				var proposalLines = await ClientDbContext.ProposalLines
					.Where(pl => pl.ProposalId == proposal.Id)
					.ToListAsync();

				var scheduleMapper = await ClientDbContext.ScheduleTaskMappings
					.ToListAsync();

				var matchingSchedules = scheduleMapper
					.Where(schedule => proposalLines
						.Any(pl => pl.EstimateCategoryId == schedule.EstimateCategoryId))
					.ToList();

				var constructionTasks = await ClientDbContext.ConstructionTasks
					.Where(ct => matchingSchedules.Select(ms => ms.ConstructionTaskId).Contains(ct.Id))
					.OrderBy(ct => ct.Sequence)
					.ToListAsync();

				projectScheduleDTO.Tasks = mapper.Map<List<ProjectScheduleTaskDto>>(constructionTasks);
			}
			else 
			{
				var defaultConstructionTasks = await ClientDbContext.ConstructionTasks
					.OrderBy(ct => ct.Sequence)
					.ToListAsync();

				projectScheduleDTO.Tasks = mapper.Map<List<ProjectScheduleTaskDto>>(defaultConstructionTasks);
			}
		}
		#endregion
	}
}
