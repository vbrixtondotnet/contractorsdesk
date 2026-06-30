using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IEmailTemplateService : IBaseService
	{
		Task<List<EmailTemplateDto>> GetEmailTemplates();
		Task<EmailTemplateDto> SaveEmailTemplate(EmailTemplatePayload model);
	}
}
