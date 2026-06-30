using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;
using InvoiceItemModel = ContractorsDesk.Core.ApiPayloadModels.InvoiceItemModel;

namespace ContractorsDesk.Services.Mappers
{
	public class InvoiceMapper : Profile
	{
		public InvoiceMapper()
		{
			CreateMap<Invoice, InvoiceDto>().ReverseMap();
			CreateMap<InvoiceItem, InvoiceItemDto>().ReverseMap();
			CreateMap<InvoicePayload, Invoice>()
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore());
			CreateMap<InvoiceItemModel, InvoiceItem>()
				.ForMember(dest => dest.Sequence, opt => opt.Ignore())
				.ForMember(dest => dest.InvoiceId, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.DateUpdated, opt => opt.Ignore());
			CreateMap<InvoiceModel, InvoicePayload > ();
			CreateMap<Core.Models.InvoiceItemModel, InvoiceItemModel>();
		}
	}
}
