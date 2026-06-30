using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class CustomerMapper : Profile
	{
		public CustomerMapper()
		{
			CreateMap<Qbcustomer, QbCustomerDto>();
			CreateMap<Qbcustomer, CustomerDto>();
			CreateMap<Qbcustomer, CustomerShortDetailsDto>();
			CreateMap<CustomerDto, ProjectClientDto>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FullName))
				.ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State))
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.ProjectId, opt => opt.Ignore());
		}
	}
}
