using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services.Mappers
{
	public class EmailTemplateMapper : Profile
	{
		public EmailTemplateMapper()
		{
			CreateMap<EmailTemplate, EmailTemplateDto>().ReverseMap();
			CreateMap<EmailTemplatePayload, EmailTemplate>();
		}
	}
}
