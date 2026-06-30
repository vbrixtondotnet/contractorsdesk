using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ProposalProjectMapper : Profile
	{
		public ProposalProjectMapper()
		{
			CreateMap<ProposalProject, ProposalProjectDto>();
			CreateMap<ProposalProjectModel, ProposalProject>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
		}

	}
}
