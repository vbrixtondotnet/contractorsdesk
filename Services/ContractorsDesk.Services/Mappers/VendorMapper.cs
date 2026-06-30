using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class VendorMapper : Profile
	{
		public VendorMapper()
		{
			CreateMap<Vendor, VendorDto>().ReverseMap();
			CreateMap<VendorPayload, Vendor>()
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
		}
	}
}
