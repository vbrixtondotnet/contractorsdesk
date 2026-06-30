using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ActivityStreamMapper : Profile
	{
		public ActivityStreamMapper()
		{
			CreateMap<ActivityStream, ActivityStreamDto>();
			CreateMap<ActivityStreamItem, ActivityStream>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
		}
	}
}
