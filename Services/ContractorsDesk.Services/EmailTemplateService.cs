using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.Services
{
    public class EmailTemplateService : BaseService, IEmailTemplateService
	{
		public EmailTemplateService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext)
		{ }

		public async Task<List<EmailTemplateDto>> GetEmailTemplates()
		{
			var emailTemplates = await ClientDbContext.EmailTemplates
				.AsNoTracking()
				.ToListAsync();

			return mapper.Map<List<EmailTemplateDto>>(emailTemplates);
		}

		public async Task<EmailTemplateDto> SaveEmailTemplate(EmailTemplatePayload model)
		{
			EmailTemplate? dbEmailTemplate = null;

			if (!model.IsNew)
			{
				dbEmailTemplate = await ClientDbContext.EmailTemplates.FirstOrDefaultAsync(s => s.Id == model.Id);

				// For future refactor | when saving, avoid updating email type to null
				var emailType = dbEmailTemplate.EmailType;

				if (dbEmailTemplate == null)
					throw new KeyNotFoundException($"EmailTemplate with Id {model.Id} not found.");

				mapper.Map(model, dbEmailTemplate);
				dbEmailTemplate.EmailType = emailType;
				dbEmailTemplate.DateModified = DateTime.UtcNow;
				dbEmailTemplate.ModifiedById = this.UserId;

				ClientDbContext.EmailTemplates.Update(dbEmailTemplate);
			}
			else
			{
				dbEmailTemplate = mapper.Map<EmailTemplate>(model);
				dbEmailTemplate.Id = Guid.NewGuid();
				dbEmailTemplate.OwnerId = this.UserId;
				dbEmailTemplate.DateCreated = DateTime.UtcNow;
				dbEmailTemplate.CreatedById = this.UserId;
				dbEmailTemplate.DateModified = DateTime.UtcNow;
				dbEmailTemplate.ModifiedById = this.UserId;

				ClientDbContext.EmailTemplates.Add(dbEmailTemplate);
			}

			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<EmailTemplateDto>(dbEmailTemplate);
		}
	}
}
