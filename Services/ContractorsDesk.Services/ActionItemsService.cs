using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Pipelines.Sockets.Unofficial.Arenas;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.Services
{
	public class ActionItemsService : BaseService, IActionItemsService
	{
		private readonly ICacheService cacheService;
		#region Public Methods
		public ActionItemsService(
			ICacheService cacheService,
			ClientDbContext clientDbContext, 
			MasterDbContext masterDbContext,
			IMapper mapper)
			: base(mapper, clientDbContext,masterDbContext) 
		{
			this.cacheService = cacheService;
		}
		public async Task<ActionItemDto> GetActionItemAsync(int id)
		{
			var dbActionItem = await ClientDbContext.ActionItems
				.Where(a => a.Id == id)
				.Include(a => a.Project)
				.Include(a => a.ActionType)
				.Include(a => a.AcceptedByNavigation)
				.Include(a=> a.CreatedByNavigation)
				.Include(a => a.ActionItemsSupervisors)
					.ThenInclude(a => a.Supervisor)
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (dbActionItem == null) throw new Exception("Action Item not found.");

			var actionItemDto = this.mapper.Map<ActionItemDto>(dbActionItem);

			if(dbActionItem.CreatedBy != 0)
			{
				actionItemDto.CreatedBy = this.mapper.Map<ApplicationUserShortDetailsDto>(dbActionItem.CreatedByNavigation);
			}

			if (dbActionItem.ActionItemsSupervisors.Any())
			{
				actionItemDto.Supervisors = this.mapper.Map<List<ApplicationUserShortDetailsDto>>(dbActionItem.ActionItemsSupervisors.Select(a => a.Supervisor).ToList());
			}

			if (dbActionItem.AcceptedByNavigation != null)
			{
				actionItemDto.AcceptedBy = this.mapper.Map<ApplicationUserShortDetailsDto>(dbActionItem.AcceptedByNavigation);
			}

			return actionItemDto;
		}
		public async Task<ActionItemShortDetailsDto> GetActionItemShortDetailsAsync(int id, bool includeComments = true)
		{
			var dbActionItem = await ClientDbContext.ActionItems
				.Where(a => a.Id == id)
				.Include(a => a.Project)
				.Include(a => a.ActionType)
				.Include(a => a.CreatedByNavigation)
				.Include(a => a.ActionItemsSupervisors)
					.ThenInclude(a => a.Supervisor)
				.Include(a => a.ActionItemComments)
					.ThenInclude(a => a.CreatedByNavigation)
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (dbActionItem != null && includeComments && dbActionItem.ActionItemComments != null)
			{
				dbActionItem.ActionItemComments = dbActionItem.ActionItemComments
					.OrderByDescending(c => c.DateCreated)
					.ToList();
			}

			if (dbActionItem == null) throw new Exception("Action Item not found.");

			var actionItemDto = this.mapper.Map<ActionItemShortDetailsDto>(dbActionItem);

			return actionItemDto;
		}
		public override async Task<T> CreateAsync<T>(object param)
		{
			if (param is ActionItemPayload actionItemPayload)
			{
				var dbActionItem = this.mapper.Map<ActionItem>(actionItemPayload);
				dbActionItem.Status = actionItemPayload.IsAiCreated ? (int)ActionItemStatus.ForReview : (int)ActionItemStatus.NotStarted;
				dbActionItem.Source = actionItemPayload.IsAiCreated ? (int)ActionItemSource.AI : (int)ActionItemSource.Manual;

				foreach (var supervisorId in actionItemPayload.Supervisors)
				{
					dbActionItem.ActionItemsSupervisors.Add(new ActionItemsSupervisor { Id = Guid.NewGuid(), SupervisorId = supervisorId });
				}

				await this.HandleActionType(actionItemPayload, dbActionItem);

				ClientDbContext.ActionItems.Add(dbActionItem);
				await ClientDbContext.SaveChangesAsync();

				return this.mapper.Map<T>(await GetActionItemSummaryViewByIdAsync(dbActionItem.Id));
			}
			else
			{
				throw new ArgumentException("Invalid payload type for CreateAsync");
			}
		}
		public async Task<List<ActionItemDto>> GetActionItemsAsync(int? supervisorId)
		{
			var dbActionItems = await ClientDbContext.ActionItemsSupervisors
				.Where(a => !supervisorId.HasValue || a.SupervisorId == supervisorId)
				.Include(a => a.ActionItem)
					.ThenInclude(p => p.ActionType)
                .Include(a => a.ActionItem)
					.ThenInclude(p => p.Project)
						.ThenInclude(p => p.Proposals)
							.ThenInclude(prop => prop.ProposalLines)
				.Include(a => a.ActionItem)
					.ThenInclude(p => p.Project)
						.ThenInclude(p => p.ProjectSchedules)
                .ThenInclude(sched => sched.ProjectScheduleTasks)
                .Include(a => a.ActionItem)
					.ThenInclude(a => a.ActionItemsSupervisors)
						.ThenInclude(a=> a.Supervisor)
                .Include(a => a.ActionItem)
					.ThenInclude(a => a.ActionItemCostChanges)
				.Include(a => a.ActionItem)
					.ThenInclude(a => a.ActionItemScheduleChanges)
                .Select(a => a.ActionItem)
				.Distinct()
				.Where(p => !p.IsDeleted)
				.OrderByDescending(p => p.DateCreated)
				.ThenBy(p => p.Status)
				.AsNoTracking()
				.ToListAsync();

            var actionItemDtos = this.mapper.Map<List<ActionItemDto>>(dbActionItems);

            foreach (var dto in actionItemDtos)
            {
                var dbActionItem = dbActionItems.FirstOrDefault(ai => ai.Id == dto.Id);
                if (dbActionItem == null) continue;

                var actionType = (ActionTypes)dbActionItem.ActionTypeId;

                if (actionType == ActionTypes.CostChange)
                {
                    var proposal = dbActionItem.Project.Proposals.FirstOrDefault();
                    var proposalId = proposal?.Id;
                    var costChange = dbActionItem.ActionItemCostChanges.FirstOrDefault();
                    var estimateCategoryId = costChange?.EstimateCategoryId ?? Guid.Empty;

                    var estimateCategoryName = proposal?.ProposalLines
                        .FirstOrDefault(pl => pl.EstimateCategoryId == estimateCategoryId)?.Name ?? string.Empty;

                    dto.CostChange.LineItem = estimateCategoryName;
                }
                else if (actionType == ActionTypes.ScheduleChange)
                {
                    var schedule = dbActionItem.Project.ProjectSchedules.FirstOrDefault();
                    var scheduleChange = dbActionItem.ActionItemScheduleChanges.FirstOrDefault();
                    var constructionTaskId = scheduleChange?.ConstructionTaskId ?? Guid.Empty;

                    var task = schedule?.ProjectScheduleTasks.FirstOrDefault(t => t.Id == constructionTaskId);
                    dto.ScheduleChange.ConstructionTaskName = task?.Name ?? string.Empty;
                }
            }

			return actionItemDtos;

		}
		public async Task<List<ActionItemDto>> GetActionItemsByStatusAsync(int? supervisorId, int statusId)
		{
			var dbActionItems = await ClientDbContext.ActionItemsSupervisors
				.Where(a => !supervisorId.HasValue || a.SupervisorId == supervisorId)
				.Include(a => a.ActionItem)
					.ThenInclude(p => p.ActionType)
				.Include(a => a.ActionItem)
					.ThenInclude(p => p.Project)
						.ThenInclude(p => p.Proposals)
							.ThenInclude(prop => prop.ProposalLines)
				.Include(a => a.ActionItem)
					.ThenInclude(p => p.Project)
						.ThenInclude(p => p.ProjectSchedules)
				.ThenInclude(sched => sched.ProjectScheduleTasks)
				.Include(a => a.ActionItem)
					.ThenInclude(a => a.ActionItemsSupervisors)
						.ThenInclude(a => a.Supervisor)
				.Include(a => a.ActionItem)
					.ThenInclude(a => a.ActionItemCostChanges)
				.Include(a => a.ActionItem)
					.ThenInclude(a => a.ActionItemScheduleChanges)
				.Select(a => a.ActionItem)
				.Distinct()
				.Where(p => !p.IsDeleted && p.Status == statusId)
				.OrderByDescending(p => p.DateCreated)
				.ThenBy(p => p.Status)
				.AsNoTracking()
				.ToListAsync();

			var actionItemDtos = this.mapper.Map<List<ActionItemDto>>(dbActionItems);

			foreach (var dto in actionItemDtos)
			{
				var dbActionItem = dbActionItems.FirstOrDefault(ai => ai.Id == dto.Id);
				if (dbActionItem == null) continue;

				var actionType = (ActionTypes)dbActionItem.ActionTypeId;

				if (actionType == ActionTypes.CostChange)
				{
					var proposal = dbActionItem.Project.Proposals.FirstOrDefault();
					var proposalId = proposal?.Id;
					var costChange = dbActionItem.ActionItemCostChanges.FirstOrDefault();
					var estimateCategoryId = costChange?.EstimateCategoryId ?? Guid.Empty;

					var estimateCategoryName = proposal?.ProposalLines
						.FirstOrDefault(pl => pl.EstimateCategoryId == estimateCategoryId)?.Name ?? string.Empty;

					dto.CostChange.LineItem = estimateCategoryName;
				}
				else if (actionType == ActionTypes.ScheduleChange)
				{
					var schedule = dbActionItem.Project.ProjectSchedules.FirstOrDefault();
					var scheduleChange = dbActionItem.ActionItemScheduleChanges.FirstOrDefault();
					var constructionTaskId = scheduleChange?.ConstructionTaskId ?? Guid.Empty;

					var task = schedule?.ProjectScheduleTasks.FirstOrDefault(t => t.Id == constructionTaskId);
					dto.ScheduleChange.ConstructionTaskName = task?.Name ?? string.Empty;
				}
			}

			return actionItemDtos;

		}
		public async Task<ActionItemSummaryViewDto> GetActionItemSummaryViewByIdAsync(int id)
		{
			var dbActionItem = await ClientDbContext.VwActionItemsSummaries
				.FirstOrDefaultAsync(ac => ac.Id == id) ?? throw new ArgumentException("Action Item cannot be found");

			var actionItemSupervisors = await ClientDbContext.ActionItemsSupervisors
				.Where(a => a.ActionItemId == dbActionItem.Id)
				.Include(a => a.Supervisor)
				.ToListAsync();

			var actionItemCreator = await ClientDbContext.Users
				.FirstOrDefaultAsync(a => a.Id == dbActionItem.CreatedById);

			var retval = this.mapper.Map<ActionItemSummaryViewDto>(dbActionItem);

			retval.SupervisorId = this.UserId;
			retval.AssignedSupervisors = actionItemSupervisors
					.Select(a => this.mapper.Map<ApplicationUserShortDetailsDto>(a.Supervisor))
					.ToList();

			if(actionItemCreator != null)
			{
				retval.CreatedByUser = this.mapper.Map<ApplicationUserShortDetailsDto>(actionItemCreator);
			}


			return retval;
		}
		public async Task<List<ActionItemSummaryViewDto>> GetDashboardActionItemsAsync(bool loadAll, int supervisorId, int? statusId, Guid? projectId = null)
		{
			List<VwActionItemsSummary> dbActionItems = await GetActionItemsByStatus(statusId, projectId);

			var actionItemSupervisors = await ClientDbContext.ActionItemsSupervisors
				.Where(a => dbActionItems.Select(ac=> ac.Id).ToList().Contains(a.ActionItemId))
				.Include(a => a.Supervisor)
				.ToListAsync();

			var actionItemCreators = await ClientDbContext.Users
				.Where(a => dbActionItems.Select(a => a.CreatedById).ToList().Contains(a.Id))
				.ToListAsync();

			if (!loadAll) {

				var actionItemsSupervisors = await ClientDbContext.ActionItemsSupervisors
					.Where(a => a.SupervisorId == supervisorId)
					.Include(a => a.Supervisor)
					.ToListAsync();

				var actionItemIdsPerSupervisor = actionItemsSupervisors
					.Select(a => a.ActionItemId)
					.ToList();

				dbActionItems = dbActionItems.Where(a => actionItemIdsPerSupervisor.Contains(a.Id)).ToList();
			}

			var actionItemDtos = this.mapper.Map<List<ActionItemSummaryViewDto>>(dbActionItems);
			foreach (var dto in actionItemDtos)
			{
				dto.SupervisorId = supervisorId;

				dto.AssignedSupervisors = actionItemSupervisors
					.Where(a => a.ActionItemId == dto.Id)
					.Select(a => this.mapper.Map<ApplicationUserShortDetailsDto>(a.Supervisor))
					.ToList();

				dto.CreatedByUser = actionItemCreators
					.Where(aic => aic.Id == dto.CreatedById)
					.Select(a => this.mapper.Map<ApplicationUserShortDetailsDto>(a))
					.FirstOrDefault();
			}

			return actionItemDtos;

		}
		public async Task<List<ActionItemDto>> GetAllActionItemsCreatedInLastDaysAsync(int days)
		{
			var startDate = DateTime.Now.AddDays(-days);
			var dbActionItems = await ClientDbContext.ActionItems
			.Where(a => a.DateCreated >= startDate)
				.OrderBy(p => p.DateCreated)
				.Include(p => p.ActionType)
				.Include(p => p.Project)
				.Include(p => p.ActionItemsSupervisors)
				.Include(p => p.ActionItemCostChanges)
				.Include(p => p.ActionItemScheduleChanges)
				.ToListAsync();

			return this.mapper.Map<List<ActionItemDto>>(dbActionItems);
		}
		public async Task<List<ActionItemDto>> GetActionItemsCreatedSinceAsync(DateTime? dateFrom = null, Guid? projectId = null)
		{
			var query = ClientDbContext.ActionItems
				.Include(p => p.ActionType)
				.Include(p => p.Project)
				.Include(p => p.ActionItemsSupervisors)
				.Include(p => p.ActionItemCostChanges)
				.Include(p => p.ActionItemScheduleChanges)
				.AsQueryable();

			if (dateFrom != null)
				query = query.Where(a => a.DateCreated >= dateFrom.Value);

			if (projectId != null)
				query = query.Where(a => a.ProjectId == projectId.Value);

			var dbActionItems = await query.OrderBy(p => p.DateCreated).ToListAsync();

			return this.mapper.Map<List<ActionItemDto>>(dbActionItems);
		}
		public async Task<List<ActionTypeDto>> GetActionTypesAsync()
		{
			List<ActionTypeDto>? retval = null;
			var cacheKey = $"ActionTypes";
			retval = await cacheService.GetFromCache<List<ActionTypeDto>>(cacheKey);

			if(retval == null)
			{
				var dbActionTypes = await ClientDbContext.ActionTypes.Where(p => !p.IsDeleted).ToListAsync();
				retval  = this.mapper.Map<List<ActionTypeDto>>(dbActionTypes);
				await cacheService.SetAsync(cacheKey, retval);
			}

			return retval;
		}
		public async Task<List<ActionItemSummaryViewDto>> GetByProjectIdAsync(Guid projectId, int? userId = null)
		{
			List<ActionItemSummaryViewDto>? retval = new List<ActionItemSummaryViewDto>();
			var cacheKey = $"ActionItemsByProject_{projectId}";
			retval = await cacheService.GetFromCache<List<ActionItemSummaryViewDto>>(cacheKey);

			if (retval == null)
			{
                var dbActionItems = await ClientDbContext.VwActionItemsSummaries
				.Where(p=>p.ProjectId == projectId)
                .OrderByDescending(p => p.DateCreated)
                .ToListAsync();

				var actionItemCreators = await ClientDbContext.Users
					.Where(a => dbActionItems.Select(ac => ac.CreatedById).ToList().Contains(a.Id))
					.ToListAsync();

				if (userId != null)
                {
                    var actionItemsSupervisors = await ClientDbContext.ActionItemsSupervisors
                        .Where(a => a.SupervisorId == userId)
                        .Include(a => a.Supervisor)
                        .ToListAsync();

                    var actionItemIdsPerSupervisor = actionItemsSupervisors
                        .Select(a => a.ActionItemId)
                        .ToList();

                    dbActionItems = dbActionItems.Where(a => actionItemIdsPerSupervisor.Contains(a.Id)).ToList();
                }

                var actionItemSupervisors = await ClientDbContext.ActionItemsSupervisors
                    .Where(a => dbActionItems.Select(ac => ac.Id).ToList().Contains(a.ActionItemId))
                    .Include(a => a.Supervisor)
                    .ToListAsync();

                retval = this.mapper.Map<List<ActionItemSummaryViewDto>>(dbActionItems);
                foreach (var dto in retval)
                {

                    dto.AssignedSupervisors = actionItemSupervisors
                        .Where(a => a.ActionItemId == dto.Id)
                        .Select(a => this.mapper.Map<ApplicationUserShortDetailsDto>(a.Supervisor))
                        .ToList();

					dto.CreatedByUser = actionItemCreators
						.Where(aic => aic.Id == dto.CreatedById)
						.Select(a => this.mapper.Map<ApplicationUserShortDetailsDto>(a))
						.FirstOrDefault();
				}

				await cacheService.SetAsync(cacheKey, retval);
            }

			return retval;
		}
		public async Task<List<ActionItemDto>> CreateActionItems(List<ActionItemPayload> payload)
		{
			// add more logic here to handle not found projects and supervisors
			var dbActionItems = new List<ActionItem>();
			foreach (var item in payload)
			{
				var dbActionItem = this.mapper.Map<ActionItem>(item);

				foreach (var supervisorId in item.Supervisors)
				{
					dbActionItem.ActionItemsSupervisors.Add(new ActionItemsSupervisor { Id = Guid.NewGuid(), SupervisorId = supervisorId });
				}

				dbActionItems.Add(dbActionItem);

				await this.HandleActionType(item, dbActionItem);
			}

			ClientDbContext.ActionItems.AddRange(dbActionItems);
			await ClientDbContext.SaveChangesAsync();
			return this.mapper.Map<List<ActionItemDto>>(dbActionItems);
		}
		public async Task<ActionItemSummaryViewDto> CreateNote(ActionItemNotePayload payload)
		{
			var actionItem = new ActionItem
			{
				Title = payload.Title,
				Description = payload.Description,
				ActionTypeId = (int)ActionTypes.Note,
				Source = (int)ActionItemSource.Manual,
			};

			actionItem.ActionItemsSupervisors.Add(new ActionItemsSupervisor
			{
				Id = Guid.NewGuid(),
				SupervisorId = payload.AssignedTo
			});

			ClientDbContext.ActionItems.Add(actionItem);
			await ClientDbContext.SaveChangesAsync();

			var actionItemSummary = await this.GetActionItemSummaryViewByIdAsync(actionItem.Id);
			actionItemSummary.SupervisorId = payload.AssignedTo;

			return actionItemSummary;
		}
		public async Task<ActionItemSummaryViewDto> AcceptActionItemAsync(int id)
		{
			var dbActionItem = await ClientDbContext.ActionItems
				.FirstOrDefaultAsync(a => a.Id == id);

			var requiresClientApproval = dbActionItem.ActionTypeId == (int)ActionTypes.CostChange ||
										 dbActionItem.ActionTypeId == (int)ActionTypes.ScheduleChange ||
										 dbActionItem.ActionTypeId == (int)ActionTypes.GeneralChangeOrder;

			dbActionItem.Status = requiresClientApproval ? (int)ActionItemStatus.PendingClientResponse : (int)ActionItemStatus.InProgress;
			dbActionItem.AcceptedBy = this.UserId;
			await ClientDbContext.SaveChangesAsync();

			return await GetActionItemSummaryViewByIdAsync(dbActionItem.Id);
		}
		public async Task<ActionItemSummaryViewDto> CompleteActionItemAsync(int id)
		{
			var dbActionItem = await ClientDbContext.ActionItems
				.FirstOrDefaultAsync(a => a.Id == id) ?? throw new Exception("Action Item not found.");

			dbActionItem.Status = (int)ActionItemStatus.Completed;
			await ClientDbContext.SaveChangesAsync();

			var cacheKey = $"ActionItemsByProject_{dbActionItem.ProjectId}";
			await cacheService.RemoveAsync(cacheKey);

			return await GetActionItemSummaryViewByIdAsync(dbActionItem.Id);
		}
		public async Task<ActionItemSummaryViewDto> ApproveActionItemAsync(ActionItemSummaryViewDto actionItem)
		{
			var dbActionItem = await ClientDbContext.ActionItems.FirstOrDefaultAsync(a => a.Id == actionItem.Id);
			
			switch (actionItem.ActionTypeId)
			{
				case (int)ActionTypes.CostChange:
					await ApplyCostChange(actionItem);
					break;
				case (int)ActionTypes.ScheduleChange:
					await ApplyScheduleChange(actionItem);
					break;
				case (int)ActionTypes.GeneralChangeOrder:
					if (actionItem.CostChangeItemId != null && actionItem.Amount != null)
					{
						await ApplyCostChange(actionItem);
					}
					if (actionItem.ScheduleChangeItemId != null && actionItem.NoOfDays != null)
					{
						await ApplyScheduleChange(actionItem);
					}
					break;
			}

			dbActionItem.Status = (int)ActionItemStatus.ClientApproved;
			await ClientDbContext.SaveChangesAsync();

			var retval = await GetActionItemSummaryViewByIdAsync(actionItem.Id);
			retval.ProposalId = actionItem.ProposalId;

			return retval;
		}
		public async Task<ActionItemSummaryViewDto> AcknowledgeActionItemAsync(ActionItemSummaryViewDto actionItem)
		{
			var dbActionItem = await ClientDbContext.ActionItems.FirstOrDefaultAsync(a => a.Id == actionItem.Id);

			switch (actionItem.ActionTypeId)
			{
				case (int)ActionTypes.CostChange:
					await ApplyCostChange(actionItem);
					break;
				case (int)ActionTypes.ScheduleChange:
					await ApplyScheduleChange(actionItem);
					break;
				case (int)ActionTypes.GeneralChangeOrder:
					if (actionItem.CostChangeItemId != null && actionItem.Amount != null)
					{
						await ApplyCostChange(actionItem);
					}
					if (actionItem.ScheduleChangeItemId != null && actionItem.NoOfDays != null)
					{
						await ApplyScheduleChange(actionItem);
					}
					break;
			}

			dbActionItem.Status = (int)ActionItemStatus.ClientAcknowledged;
			await ClientDbContext.SaveChangesAsync();

			var retval = await GetActionItemSummaryViewByIdAsync(actionItem.Id);
			retval.ProposalId = actionItem.ProposalId;

			return retval;
		}
		public async Task SetActionItemStatusAsync(int actionItemId, ActionItemStatus status)
		{
			var dbActionItem = await ClientDbContext.ActionItems.FirstOrDefaultAsync(a => a.Id == actionItemId);
			dbActionItem.Status = (int)status;
			await ClientDbContext.SaveChangesAsync();
		}
		public async Task<ActionItemCostChangeDto> GetActionItemCostChangeAsync(int actionItemId)
		{
			var dbActionItem = await ClientDbContext.ActionItems
				.Where(a => a.Id == actionItemId)
				.Include(a => a.ActionItemCostChanges)
				.Include(a => a.Project)
					.ThenInclude(p => p.Proposals)
					.ThenInclude(prop => prop.Client)
				.Include(a => a.Project)
					.ThenInclude(p => p.Proposals)
					.ThenInclude(prop => prop.ProposalLines)
				.FirstOrDefaultAsync();

			var actionItemCostChange = this.mapper.Map<ActionItemCostChangeDto>(dbActionItem.ActionItemCostChanges.FirstOrDefault());

			var proposal = dbActionItem.Project.Proposals.FirstOrDefault();
			var proposalLines = dbActionItem.Project.Proposals.SelectMany(p => p.ProposalLines).ToList();
			var estimateCategory = proposalLines.FirstOrDefault(pl => pl.EstimateCategoryId == actionItemCostChange.EstimateCategoryId);
			var latestProposalItemHistory = await ClientDbContext.ProposalLinesHistories
			.Where(p => p.ProposalId == proposal.Id && p.EstimateCategoryId == estimateCategory.EstimateCategoryId)
			.OrderByDescending(p => p.ChangeDate)
			.FirstOrDefaultAsync();

			actionItemCostChange.OriginalAmount = latestProposalItemHistory != null ? latestProposalItemHistory.Amount : estimateCategory.Amount;
			actionItemCostChange.EstimateCategory = estimateCategory.Name;
			actionItemCostChange.Client = mapper.Map<ProposalClientDto>(proposal.Client);
			actionItemCostChange.Project = mapper.Map<ProposalProjectDetailsDto>(dbActionItem.Project);
			actionItemCostChange.ActionItem = mapper.Map<ActionItemDto>(dbActionItem);
			return actionItemCostChange;
		}
		public async Task<ActionItemDto> ArchiveActionItemAsync(int id, bool toArchive = false)
		{
			var dbActionItem = await ClientDbContext.ActionItems
				.Where(a => a.Id == id)
				.Include(a => a.Project)
					.ThenInclude(p => p.Proposals)
					.ThenInclude(prop => prop.Client)
				.Include(a => a.Project)
					.ThenInclude(p => p.Proposals)
					.ThenInclude(prop => prop.ProposalLines)
				.Include(a => a.Project)
					.ThenInclude(p => p.ProjectSupervisors)
				.Include(a => a.ActionItemsSupervisors)
				.Include(a => a.ActionItemCostChanges)
				.Include(a => a.ActionItemScheduleChanges)
				.FirstOrDefaultAsync() ?? throw new Exception("Cannot find action item.");

			dbActionItem.IsArchived = toArchive;

			await ClientDbContext.SaveChangesAsync();

			var cacheKey = $"ActionItemsByProject_{dbActionItem.ProjectId}";
			await cacheService.RemoveAsync(cacheKey);

			var actionItemDto = this.mapper.Map<ActionItemDto>(dbActionItem);
			return actionItemDto;
		}
		public async Task<bool> DeleteActionItemAsync(int id)
		{
			var actionItem = await ClientDbContext.ActionItems.FirstOrDefaultAsync(a => a.Id == id);
			if (actionItem != null)
			{
				actionItem.IsDeleted = true;
				await ClientDbContext.SaveChangesAsync();
				return true;
			}

			return false;
		}
        public async Task<ActionItemSummaryViewDto> UpdateActionItemAsync(int id, ActionItemPayload payload)
        {
            var dbActionItem = await ClientDbContext.ActionItems
                .Include(ai => ai.ActionItemsSupervisors)
                .Include(ai => ai.ActionItemCostChanges)
                .Include(ai => ai.ActionItemScheduleChanges)
                .FirstOrDefaultAsync(ai => ai.Id == id) ?? throw new Exception("Action Item not found.");

			

            dbActionItem.Title = payload.Title;
            dbActionItem.Description = payload.Description;
			dbActionItem.DueDate = payload.DueDate;
			dbActionItem.ProjectId = payload.ProjectId;
			dbActionItem.ActionTypeId = payload.ActionTypeId;

			var supervisors = dbActionItem.ActionItemsSupervisors.ToList();
			if (payload.Supervisors != null)
			{
				ClientDbContext.ActionItemsSupervisors.RemoveRange(supervisors);
			}
			foreach (var supervisorId in payload.Supervisors)
			{
				dbActionItem.ActionItemsSupervisors.Add(new ActionItemsSupervisor { Id = Guid.NewGuid(), SupervisorId = supervisorId });
			}

			var costChange = dbActionItem.ActionItemCostChanges.FirstOrDefault();
			var scheduleChange = dbActionItem.ActionItemScheduleChanges.FirstOrDefault();

			if(costChange != null)
			{
				ClientDbContext.ActionItemCostChanges.Remove(costChange);
			}

			if(scheduleChange != null)
			{
				ClientDbContext.ActionItemScheduleChanges.Remove(scheduleChange);
			}

			await HandleActionType(payload, dbActionItem);
			await ClientDbContext.SaveChangesAsync();
			var cacheKey = $"ActionItemsByProject_{dbActionItem.ProjectId}";
			await cacheService.RemoveAsync(cacheKey);

			return await GetActionItemSummaryViewByIdAsync(id);
        }
		public async Task<ActionItemShortDetailsDto> UpdateActionItemDetails(ActionItemDetailsUpdatePayload payload)
		{
			var dbActionItem = await ClientDbContext
				.ActionItems
				.Include(a=>a.ActionItemsSupervisors)
				.FirstOrDefaultAsync(a => a.Id == payload.Id) ?? throw new Exception("Action Item not found");

			if (payload.Description != null)
			{
				dbActionItem.Description = payload.Description;
			}

			if(payload.AssignedSupervisors != null && payload.AssignedSupervisors.Any())
			{
				var dbActionItemSupervisors = dbActionItem.ActionItemsSupervisors.ToList();
				ClientDbContext.ActionItemsSupervisors.RemoveRange(dbActionItemSupervisors);

				foreach (var supervisorId in payload.AssignedSupervisors)
				{
					dbActionItem.ActionItemsSupervisors.Add(new ActionItemsSupervisor { Id = Guid.NewGuid(), SupervisorId = supervisorId });
				}
			}

			await ClientDbContext.SaveChangesAsync();
			return await GetActionItemShortDetailsAsync(payload.Id);
		}
		public async Task<bool> UpdateActionItemSupervisors(int id, ActionItemPayload payload)
		{
            var dbActionItem = await ClientDbContext.ActionItems
                .Include(ai => ai.ActionItemsSupervisors)
                .Include(ai => ai.ActionItemCostChanges)
                .Include(ai => ai.ActionItemScheduleChanges)
                .FirstOrDefaultAsync(ai => ai.Id == id);

            if (dbActionItem == null)
                return false;

            if (payload.Supervisors != null)
			{
				ClientDbContext.ActionItemsSupervisors.RemoveRange(dbActionItem.ActionItemsSupervisors);
				foreach (var supervisorId in payload.Supervisors)
				{
					dbActionItem.ActionItemsSupervisors.Add(new ActionItemsSupervisor
					{
						Id = Guid.NewGuid(),
						SupervisorId = supervisorId,
                        ActionItemId = dbActionItem.Id
                    });
				}
			}

			await ClientDbContext.SaveChangesAsync();

			var cacheKey = $"ActionItemsByProject_{dbActionItem.ProjectId}";
			await cacheService.RemoveAsync(cacheKey);

			return true;
        }
		public async Task<ActionItemCommentDto> AddCommentAsync(ActionItemCommentPayload payload)
		{
			var actionItemComment = new ActionItemComment();
			actionItemComment.ActionItemId = payload.ActionItemId;
			actionItemComment.Comment = payload.Comment;

			ClientDbContext.ActionItemComments.Add(actionItemComment);
			await ClientDbContext.SaveChangesAsync();

			var dbActionItemComment = await ClientDbContext.ActionItemComments
				.Include(a=> a.CreatedByNavigation)
				.FirstOrDefaultAsync(a => a.Id == actionItemComment.Id);

			return mapper.Map<ActionItemCommentDto>(dbActionItemComment);
		}

		#endregion

		#region Private Methods
		private async Task ApplyCostChange(ActionItemSummaryViewDto actionItem)
		{
			var proposal = await ClientDbContext.Proposals
				.FirstOrDefaultAsync(pr => pr.QbclassId == actionItem.ProjectId);

			var proposalLine = await ClientDbContext.ProposalLines
				.FirstOrDefaultAsync(pl => pl.EstimateCategoryId == actionItem.CostChangeItemId && pl.ProposalId == proposal.Id);

			var costRevision = await ClientDbContext.CostRevisions.FirstOrDefaultAsync(cr=> cr.ActionItemId == actionItem.Id);

			if (costRevision != null)
			{
				var proposalLineHistory = new ProposalLinesHistory();
				proposalLineHistory.HistoryId = Guid.NewGuid();
				proposalLineHistory.ProposalId = proposal.Id;
				proposalLineHistory.ProposalLineId = proposalLine.Id;
				//proposalLineHistory.Amount = costRevision.NewAmount;
				proposalLineHistory.Created = DateTime.Now;
				proposalLineHistory.EstimateCategoryId = proposalLine.EstimateCategoryId;
				proposalLineHistory.ParentEstimateCategoryId = proposalLine.ParentEstimateCategoryId;
				proposalLineHistory.Description = proposalLine.Description;
				proposalLineHistory.ChangeType = "Updated";
				proposalLineHistory.ChangeDate = DateTime.Now;

				ClientDbContext.ProposalLinesHistories.Add(proposalLineHistory);
			}

			actionItem.ProposalId = proposal.Id;
		}
		private async Task ApplyScheduleChange(ActionItemSummaryViewDto actionItem)
		{
			var projectSchedule = await ClientDbContext.ProjectSchedules
					.FirstOrDefaultAsync(ps => ps.ProjectId == actionItem.ProjectId);

			var projectScheduleTask = await ClientDbContext.ProjectScheduleTasks
				.FirstOrDefaultAsync(t => t.Id == actionItem.ScheduleChangeItemId && t.ProjectScheduleId == projectSchedule.Id);

			//var scheduleRevision = await clientDbContext.ScheduleRevisions.FirstOrDefaultAsync(cr => cr.ActionItemId == actionItem.Id);

			//if (scheduleRevision != null)
			//{
			//	projectScheduleTask.Duration = scheduleRevision.NewDuration;
			//	projectScheduleTask.StartDate = scheduleRevision.NewStartDate;
			//	projectScheduleTask.EndDate = scheduleRevision.NewEndDate;
			//}
		}
		private async Task HandleActionType(ActionItemPayload actionItem, ActionItem dbActionItem)
		{
			switch (actionItem.ActionTypeId)
			{
				case (int)ActionTypes.CostChange:
					await this.HandleCostChange(actionItem, dbActionItem);
					break;
				case (int)ActionTypes.ScheduleChange:
					await this.HandleScheduleChange(actionItem, dbActionItem);
					break;
				case (int)ActionTypes.ClientContact:
					await this.HandleClientContact(actionItem, dbActionItem);
					break;
				case (int)ActionTypes.SubContractorContact:
					await this.HandleSubContractorContact(actionItem, dbActionItem);
					break;
				case (int)ActionTypes.Note:
					await this.HandleNote(actionItem, dbActionItem);
					break;
				case (int)ActionTypes.Followup:
					await this.HandleFollowup(actionItem, dbActionItem);
					break;
				case (int)ActionTypes.Reminder:
					await this.HandleReminder(actionItem, dbActionItem);
					break;
                case (int)ActionTypes.GeneralChangeOrder:
                    if (actionItem.CostChangeAmount != null && actionItem.CostChangeEstimateCategoryId != null)
                    {
                        await this.HandleCostChange(actionItem, dbActionItem);
                    }
                    if (actionItem.ScheduleChangeTaskId != null && actionItem.ScheduleChangeNumberOfDays != null)
                    {
                        await this.HandleScheduleChange(actionItem, dbActionItem);
                    }
                    break;
            }
		}
		private async Task HandleCostChange(ActionItemPayload actionItem, ActionItem dbActionItem)
		{
			await Task.Run(async () => {
				if (actionItem.CostChangeAmount != null && actionItem.CostChangeEstimateCategoryId != null)
				{
					dbActionItem.ActionItemCostChanges.Add(new ActionItemCostChange
					{
						Amount = actionItem.CostChangeAmount,
						EstimateCategoryId = actionItem.CostChangeEstimateCategoryId,
						RequiresClientApproval = actionItem.CostChangeRequiresClientApproval
					});
				}
			});
			
		}
		private async Task HandleScheduleChange(ActionItemPayload actionItem, ActionItem dbActionItem)
		{
			await Task.Run(() => {
				if (actionItem.ScheduleChangeNumberOfDays != null && actionItem.ScheduleChangeTaskId != null)
				{
                    dbActionItem.ActionItemScheduleChanges.Add(new ActionItemScheduleChange
                    {
                        NoOfDays = actionItem.ScheduleChangeNumberOfDays,
                        ConstructionTaskId = actionItem.ScheduleChangeTaskId,
                        RequiresClientApproval = actionItem.ScheduleChangeRequiresClientApproval
                    });
                }
			});
			
		}
		private async Task HandleClientContact(ActionItemPayload actionItem, ActionItem dbActionItem)
		{

		}
		private async Task HandleSubContractorContact(ActionItemPayload actionItem, ActionItem dbActionItem)
		{

		}
		private async Task HandleNote(ActionItemPayload actionItem, ActionItem dbActionItem)
		{

		}
		private async Task HandleFollowup(ActionItemPayload actionItem, ActionItem dbActionItem)
		{

		}
		private async Task HandleReminder(ActionItemPayload actionItem, ActionItem dbActionItem)
		{

		}
		private async Task<List<VwActionItemsSummary>> GetActionItemsByStatus(int? statusId, Guid? projectId = null)
		{
			var dbActionItems = new List<VwActionItemsSummary>();
			if (statusId == (int)ActionItemStatus.PendingClientResponse)
			{
				dbActionItems = await ClientDbContext.VwActionItemsSummaries
								.Where(a => 
									(a.StatusId == (int)ActionItemStatus.PendingClientResponse || a.StatusId == (int)ActionItemStatus.PendingClientAcknowledgement)
									&& (projectId == null || a.ProjectId == projectId)
									&& (a.IsArchived == false)
									)
                                .OrderByDescending(p => p.DateCreated)
								.AsNoTracking()
								.ToListAsync();
			}
			else if (statusId == (int)ActionItemStatus.ClientApproved)
			{
				dbActionItems = await ClientDbContext.VwActionItemsSummaries
								.Where(a => (a.StatusId == (int)ActionItemStatus.ClientApproved || a.StatusId == (int)ActionItemStatus.ClientAcknowledged)
                                    && (projectId == null || a.ProjectId == projectId)
									&& (a.IsArchived == false))
                                .OrderByDescending(p => p.DateCreated)
								.AsNoTracking()
								.ToListAsync();
			}
			else if (statusId == (int)ActionItemStatus.Completed)
			{
				dbActionItems = await ClientDbContext.VwActionItemsSummaries
								.Where(a => (a.StatusId == (int)ActionItemStatus.Completed && !a.IsArchived)
                                    && (projectId == null || a.ProjectId == projectId)
									&& (a.IsArchived == false))
                                .OrderByDescending(p => p.DateCreated)
								.AsNoTracking()
								.ToListAsync();
			}
			else if (statusId == (int)ActionItemStatus.Archived)
			{
				dbActionItems = await ClientDbContext.VwActionItemsSummaries
								.Where(a => (a.IsArchived) && (projectId == null || a.ProjectId == projectId))
								.OrderByDescending(p => p.DateCreated)
								.AsNoTracking()
								.ToListAsync();
			}
			else
			{
				dbActionItems = await ClientDbContext.VwActionItemsSummaries
								.Where(a => (statusId == null || a.StatusId == statusId) && (projectId == null || a.ProjectId == projectId) && (a.IsArchived == false))
								.OrderByDescending(p => p.DateCreated)
								.AsNoTracking()
								.ToListAsync();
			}

			return dbActionItems;
		}
	#endregion
	}
}
