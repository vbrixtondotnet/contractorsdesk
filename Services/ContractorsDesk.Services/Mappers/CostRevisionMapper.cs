using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class CostRevisionMapper : Profile
	{
		public CostRevisionMapper()
		{
			CreateMap<CostRevision, CostRevisionDto>().ReverseMap();
			CreateMap<CostRevisionItem, CostRevisionItemDto>().ReverseMap();
		}
	}
}
