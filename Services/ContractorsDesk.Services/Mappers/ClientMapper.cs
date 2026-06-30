using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ClientMapper : Profile
	{
		public ClientMapper()
		{
			CreateMap<Client, ClientDto>();

			CreateMap<Client, ProposalClientDto>()
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name));

			CreateMap<ClientModel, Client>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
		}

	}
}
