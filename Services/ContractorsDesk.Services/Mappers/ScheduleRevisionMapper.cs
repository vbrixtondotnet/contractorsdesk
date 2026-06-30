using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ScheduleRevisionMapper : Profile
	{
		public ScheduleRevisionMapper()
		{
			CreateMap<ScheduleRevision, ScheduleRevisionDto>();
			CreateMap<ScheduleRevisionItem, ScheduleRevisionItemDto>().ReverseMap();
			CreateMap<ScheduleRevisionPayload, ScheduleRevisionItem>()
				.ForMember(dest => dest.ScheduleRevisionId, opt => opt.Ignore());

		}
	}
}
