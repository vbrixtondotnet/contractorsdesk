using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ActionItemMapper : Profile
	{
		public ActionItemMapper()
		{
			CreateMap<ActionType, ActionTypeDto>();
			CreateMap<ActionItem, ActionItemDto>()
				.ForMember(dest => dest.ActionTypeName, opt => opt.MapFrom(src => src.ActionType.Title))
				.ForMember(dest => dest.Status, opt => opt.Ignore())
				.ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.IsArchived, opt => opt.MapFrom(src => src.IsArchived))
				.ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.DateCreated))
				.ForMember(dest => dest.ProjectClient, opt => opt.Ignore())
				.ForMember(dest => dest.CostChange, opt => opt.Ignore())
				.ForMember(dest => dest.ScheduleChange, opt => opt.Ignore())
				.ForMember(dest => dest.ProposalId, opt => opt.Ignore())
				.ForMember(dest => dest.Supervisors, opt => opt.MapFrom(src => src.ActionItemsSupervisors.Select(ais=> ais.Supervisor)))
				.ForMember(dest => dest.AcceptedBy, opt => opt.MapFrom(src => src.AcceptedByNavigation))
				.ForMember(dest => dest.CostChange, opt => opt.MapFrom(src => MapActionItemCostChange(src.ActionItemCostChanges.FirstOrDefault())))
				.ForMember(dest => dest.ScheduleChange, opt => opt.MapFrom(src => MapActionItemScheduleChange(src.ActionItemScheduleChanges.FirstOrDefault())))
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

			CreateMap<ActionItemPayload, ActionItem>()
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore());

			CreateMap<ActionItem, ActionItemSummaryDto>()
				.ForMember(dest => dest.ActionTypeName, opt => opt.MapFrom(src => src.ActionType.Title))
				.ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project.Name))
				.ForMember(dest => dest.AssignedSupervisors, opt => opt.MapFrom(src => src.ActionItemsSupervisors.Select(a=> a.Supervisor)))
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore());

			CreateMap<VwActionItemsSummary, ActionItemSummaryViewDto>()
				.ForMember(dest => dest.SupervisorId, opt => opt.Ignore());

			CreateMap<ActionItemCostChange, ActionItemCostChangeDto>()
				.ForMember(dest => dest.OriginalAmount, opt => opt.Ignore())
				.ForMember(dest => dest.Project, opt => opt.Ignore())
				.ForMember(dest => dest.Client, opt => opt.Ignore())
				.ForMember(dest => dest.EstimateCategory, opt => opt.Ignore());

			CreateMap<ActionItemScheduleChange, ActionItemScheduleChangeDto>()
				.ForMember(dest => dest.OriginalNoOfDays, opt => opt.Ignore());

			CreateMap<ActionItemComment, ActionItemCommentDto>()
				.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation));

			CreateMap<ActionItem, ActionItemShortDetailsDto> ()
				.ForMember(dest => dest.ProjectName, opt => opt.MapFrom(src => src.Project != null ? src.Project.Name : string.Empty))
				.ForMember(dest => dest.CreatedByUser, opt => opt.MapFrom(src => src.CreatedByNavigation))
				.ForMember(dest => dest.Comments, opt => opt.MapFrom(src => src.ActionItemComments))
                .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.AssignedSupervisors, opt => opt.MapFrom(src => src.ActionItemsSupervisors.Select(a => a.Supervisor).ToList()));
		}

		private ActionItemCostChangeDto? MapActionItemCostChange(ActionItemCostChange? actionItemCostChange)
		{
			if (actionItemCostChange == null) return null;

			var retval = new ActionItemCostChangeDto();
			retval.Id = actionItemCostChange.Id;
			retval.ActionItemId = actionItemCostChange.ActionItemId;
			retval.Amount = actionItemCostChange.Amount.Value;
			retval.EstimateCategoryId = actionItemCostChange.EstimateCategoryId;
			retval.RequiresClientApproval = actionItemCostChange.RequiresClientApproval;
			return retval;
		}
		private ActionItemScheduleChangeDto? MapActionItemScheduleChange(ActionItemScheduleChange? actionItemScheduleChange)
		{
			if (actionItemScheduleChange == null) return null;

			var retval = new ActionItemScheduleChangeDto();
			retval.Id = actionItemScheduleChange.Id;
			retval.ActionItemId = actionItemScheduleChange.ActionItemId;
			retval.ConstructionTaskId = actionItemScheduleChange.ConstructionTaskId;
			retval.RequiresClientApproval = actionItemScheduleChange.RequiresClientApproval;
			retval.NoOfDays = actionItemScheduleChange.NoOfDays;
			return retval;
		}
	}
}
