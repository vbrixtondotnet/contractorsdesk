using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.Services.Mappers
{
	public class ConstructionTaskMapper : Profile
	{
		public ConstructionTaskMapper()
		{
			CreateMap<ConstructionTask, ConstructionTaskShortDto>();
			CreateMap<ConstructionTask, Core.Dto.ConstructionTaskDto>()
				.ForMember(dest => dest.ConstructionTaskId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.ParentTaskName, opt => opt.MapFrom(src => src.ParentTask != null ? src.ParentTask.Name : string.Empty));
		}
	}
}
