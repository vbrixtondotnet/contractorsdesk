using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.Services.Mappers
{
	public class ProjectScheduleMapper : Profile
	{
		public ProjectScheduleMapper()
		{
			CreateMap<ConstructionTask, ProjectScheduleTaskDto>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.ConstructionTaskId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Pred1, opt => opt.MapFrom(src => src.ParentTaskId));
				//.ForMember(dest => dest.Pred2, opt => opt.MapFrom(src => src.Pred2Id))
				//.ForMember(dest => dest.Pred3, opt => opt.MapFrom(src => src.Pred3Id))
				//.ForMember(dest => dest.Lag1, opt => opt.MapFrom(src => src.Pred1Lag))
				//.ForMember(dest => dest.Lag2, opt => opt.MapFrom(src => src.Pred2Lag))
				//.ForMember(dest => dest.Lag3, opt => opt.MapFrom(src => src.Pred3Lag));

			CreateMap<ProjectScheduleTaskDto, ProjectScheduleTask>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.ConstructionTaskId, opt => opt.MapFrom(src => src.ConstructionTaskId))
				.ForMember(dest => dest.ProjectScheduleId, opt => opt.Ignore())
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
				.ForMember(dest => dest.StartDate,
				 opt => opt.MapFrom(src => src.StartDate.HasValue
										 ? (DateOnly?)DateOnly.FromDateTime(src.StartDate.Value)
										 : null))
				.ForMember(dest => dest.EndDate,
				 opt => opt.MapFrom(src => src.EndDate.HasValue
										 ? (DateOnly?)DateOnly.FromDateTime(src.EndDate.Value)
										 : null))

				.ForMember(dest => dest.ProjectSchedule, opt => opt.Ignore());

			CreateMap<ProjectScheduleTask, ProjectScheduleTaskDto>()
				.ForMember(dest => dest.StartDate,
				 opt => opt.MapFrom(src => src.StartDate.ToDateTime(TimeOnly.MinValue)))
				.ForMember(dest => dest.EndDate,
				 opt => opt.MapFrom(src => src.EndDate.ToDateTime(TimeOnly.MinValue)));

			CreateMap<ProjectScheduleDto, ProjectSchedule>()
				.ForMember(dest => dest.ProjectId, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate.Value)));

			CreateMap<ProjectSchedulePayload, ProjectSchedule>()
				.ForMember(dest => dest.ProjectId, opt => opt.Ignore())
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
				.ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.StartDate.Value)));

			CreateMap<ProjectScheduleDelay, ProjectScheduleDelayDto>()
                .ForMember(dest => dest.Task, opt => opt.MapFrom(src => src.Task))
				.ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start.ToDateTime(TimeOnly.MinValue)));

			CreateMap<ProjectScheduleDelayDto, ProjectScheduleDelay>()
				.ForMember(dest => dest.Start, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Start)));


		}
	}
}
