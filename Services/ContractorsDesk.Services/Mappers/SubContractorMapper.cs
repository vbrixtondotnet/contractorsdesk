using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class SubContractorMapper : Profile
	{
		public SubContractorMapper()
		{
			CreateMap<SubContractor, SubContractorDto>().ReverseMap();
			CreateMap<SubContractorPayloadModel, SubContractor>()
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
		}
	}
}
