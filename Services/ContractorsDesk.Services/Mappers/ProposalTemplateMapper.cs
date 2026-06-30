using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class ProposalTemplateMapper : Profile
	{
		public ProposalTemplateMapper()
		{
		
			CreateMap<ProposalLineItemDto, ProposalTemplateLineItemDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ItemId))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ItemName))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.CategoryId))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.ItemId))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));


			CreateMap<ProposalTemplate, ProposalTemplateDto>();

			CreateMap<ProposalTemplateDto, ProposalTemplate>()
				.ForMember(dest => dest.ProposalTemplatesLineItems, opt => opt.MapFrom(src => src.Categories));

			CreateMap<ProposalTemplatesLineItem, ProposalTemplateLineItemDto>();

			CreateMap<ProposalTemplatesLineItem, ProposalTemplateLineItemDto>()
				.ForMember(dest => dest.LineItems, opt => opt.MapFrom(src => src.InverseParent));

			CreateMap<ProposalTemplateLineItemDto, ProposalLine>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.ProposalId, opt => opt.Ignore())
				.ForMember(dest => dest.ParentEstimateCategoryId, opt => opt.MapFrom(src => src.ParentId))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.EstimateCategoryId))
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
				.ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage));

			CreateMap<ProposalTemplateLineItemModel, ProposalLine>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.ProposalId, opt => opt.Ignore())
				.ForMember(dest => dest.ParentEstimateCategoryId, opt => opt.MapFrom(src => src.ParentId))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.EstimateCategoryId))
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
				.ForMember(dest => dest.SqFootLocked, opt => opt.MapFrom(src => src.SqFootLocked))
				.ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage));

			CreateMap<ProposalLine, ProposalTemplateLineItemDto>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentEstimateCategoryId))
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount));

			CreateMap<EstimateCategory, ProposalTemplateLineItemDto>()
				.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.ParentId, opt => opt.Ignore())
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence));

			CreateMap<ProposalLine, ProposalLineItemDto>()
				.ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.EstimateCategoryId))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.ParentEstimateCategoryId))
				.ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage))
				.ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Name));

			CreateMap<ProposalTemplateItemPayload, ProposalTemplatesLineItem>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.ProposalTemplate, opt => opt.Ignore())
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Sequence, opt => opt.MapFrom(src => src.Sequence))
				.ForMember(dest => dest.EstimateCategoryId, opt => opt.MapFrom(src => src.EstimateCategoryId))
				.ForMember(dest => dest.Percentage, opt => opt.MapFrom(src => src.Percentage));

			CreateMap<ProposalTemplateUserDefaultDto, ProposalTemplateUserDefault>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.TemplateId))
				.ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
		}
	}
}
