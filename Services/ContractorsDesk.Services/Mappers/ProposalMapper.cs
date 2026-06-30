using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;

namespace ContractorsDesk.Services.Mappers
{
	public class ProposalMapper : Profile
	{
		public ProposalMapper()
		{
			CreateMap<Proposal, ProposalDto>().ReverseMap();

			CreateMap<Qbcustomer, ProposalClientDto>()
				.ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name));

			CreateMap<Qbclass, ProposalProjectDetailsDto>();

			CreateMap<Proposal, ProposalManagementDto>()
				.ForMember(dest => dest.Project, opt => opt.Ignore())
				.ForMember(dest => dest.Client, opt => opt.Ignore())
				//.ForMember(dest => dest.DateCreated, opt => opt.MapFrom(src => src.Created))
				//.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation))
				//.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedByNavigation))
				.ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByNavigation))
				.ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedByNavigation))
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date.ToShortDateString()));

			CreateMap<ProposalLine, ProposalLineItemDto>()
				.ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.EstimateCategoryId))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.ParentEstimateCategoryId))
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
				.ForMember(dest => dest.SqFoot, opt => opt.MapFrom(src => src.SqFoot))
				.ForMember(dest => dest.Multiplier, opt => opt.MapFrom(src => src.Multiplier))
				.ForMember(dest => dest.SqFootLocked, opt => opt.MapFrom(src => src.SqFootLocked))
				.ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Name));

			CreateMap<ProposalLineItemDto, ProposalLine>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.ProposalId, opt => opt.Ignore())
				.ForMember(dest => dest.ParentEstimateCategoryId, opt => opt.MapFrom(src => src.CategoryId))
				.ForMember(dest => dest.SqFootLocked, opt => opt.MapFrom(src => src.SqFootLocked))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.ItemId));
		}
	}
}
