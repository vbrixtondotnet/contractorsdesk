using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using AutoMapper;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class EmailMapper : Profile
	{
		public EmailMapper()
		{
			CreateMap<Email, SentEmailDto>()
				.ForMember(dest => dest.SentAt, opt => opt.Ignore())
				.ForMember(dest => dest.Attachments, opt => opt.MapFrom(a => a.EmailAttachments));

			CreateMap<EmailAttachment, EmailAttachmentDto>();

			CreateMap<EmailModel, Email>()
				.ForMember(dest => dest.Id, opt => opt.Ignore())
				.ForMember(dest => dest.DateCreated, opt => opt.Ignore())
				.ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
				.ForMember(dest => dest.EmailAttachments, opt => opt.Ignore());

			CreateMap<GetUserInboxSpResult, InboxMessageDto>();
		}
	}
}
