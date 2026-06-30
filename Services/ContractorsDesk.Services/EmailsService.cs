using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ContractorsDesk.Services
{
	public class EmailsService : BaseService, IEmailsService
	{
		private readonly IProjectsService projectsService;
		public EmailsService(
			IProjectsService projectsService,
			ClientDbContext clientDbContext,
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDbContext, masterDbContext, configuration) 
			{ 
				this.projectsService = projectsService;
			}

		public async Task<Email> GetEmailByMessageId(string messageId)
		{
			var email = await ClientDbContext.Emails.AsNoTracking().FirstOrDefaultAsync(e => e.MessageId == messageId);
			return email;
		}

		public async Task<List<string>> GetToEmailAddresses()
		{
			var customerEmails = await ClientDbContext.Qbcustomers
							.Where(c => !string.IsNullOrEmpty(c.Email) && c.Email.Contains("@"))
							.Select(c => c.Email)
							.ToListAsync();

			var inboxEmails = await ClientDbContext.Emails
							.Where(e => e.SenderId == null)
							.Select(e => e.From)
							.ToListAsync();

			var toEmailAddresses = customerEmails.Union(inboxEmails).Distinct().ToList();
			return toEmailAddresses;	
		}

		public async Task<Guid> SaveEmailAsync(EmailModel emailModel)
		{
			int userId = this.UserId;

			var email = mapper.Map<Email>(emailModel);
			email.Id = Guid.NewGuid();
			email.DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc();
			email.CreatedBy = userId == 0 ? 1 : 0;
			ClientDbContext.Emails.Add(email);

			if (emailModel.Attachments != null && emailModel.Attachments.Any())
			{
				foreach (var attachment in emailModel.Attachments)
				{
					var emailAttachment = new EmailAttachment
					{
						EmailId = email.Id,
						FileName = attachment.FileName,
						FileUrl = attachment.FileUrl
					};

					ClientDbContext.EmailAttachments.Add(emailAttachment);
				}
			}

			await ClientDbContext.SaveChangesAsync();
			return email.Id;
		}

		public async Task<UserInboxDto> GetUserInboxAsync(Roles role, int supervisorId, bool canManageAllProjects)
		{
			var activeProjects = await this.projectsService.GetSupervisorActiveJobs(role, supervisorId, canManageAllProjects);
			var inboxProjects = mapper.Map<List<InboxProjectDto>>(activeProjects);

			var inboxMessages = await this.ClientDbContext.GetUserInboxSpResult
			.FromSqlRaw($"EXEC spCDGetUserInbox '{supervisorId}'")
			.ToListAsync();

			var inboxMessagesDto = mapper.Map<List<InboxMessageDto>>(inboxMessages);

			foreach (var message in inboxMessagesDto) { 
				var attachments = await ClientDbContext.EmailAttachments.Where(a=> a.EmailId == message.Id).ToListAsync();
				message.Attachments = mapper.Map<List<EmailAttachmentDto>>(attachments);
			}

			foreach (var project in inboxProjects)
			{
				project.UnreadCount = inboxMessagesDto.Count(m => m.ProjectId == project.Id && !m.IsRead);
				project.TotalCount = inboxMessagesDto.Count(m => m.ProjectId == project.Id);
			}

			inboxProjects = inboxProjects.OrderByDescending(p => p.UnreadCount).ThenByDescending(p => p.TotalCount).ThenBy(p=> p.Name).ToList();

			var userInboxDto = new UserInboxDto
			{
				Projects = inboxProjects,
				Messages = inboxMessagesDto
			};

			return userInboxDto;
		}
		public async Task<UserInboxDto> GetUserSentItemsAsync(Roles role, int supervisorId, bool canManageAllProjects)
		{
			var activeProjects = await this.projectsService.GetSupervisorActiveJobs(role, supervisorId, canManageAllProjects);
			var inboxProjects = mapper.Map<List<InboxProjectDto>>(activeProjects);

			var inboxMessages = await this.ClientDbContext.GetUserInboxSpResult
			.FromSqlRaw($"EXEC spCDGetUserSentItems '{supervisorId}'")
			.ToListAsync();

			var inboxMessagesDto = mapper.Map<List<InboxMessageDto>>(inboxMessages);

			foreach (var message in inboxMessagesDto)
			{
				var attachments = await ClientDbContext.EmailAttachments.Where(a => a.EmailId == message.Id).ToListAsync();
				message.Attachments = mapper.Map<List<EmailAttachmentDto>>(attachments);
			}

			foreach (var project in inboxProjects)
			{
				project.UnreadCount = inboxMessagesDto.Count(m => m.ProjectId == project.Id && !m.IsRead);
				project.TotalCount = inboxMessagesDto.Count(m => m.ProjectId == project.Id);
			}

			inboxProjects = inboxProjects.OrderByDescending(p => p.UnreadCount).ThenByDescending(p => p.TotalCount).ThenBy(p => p.Name).ToList();

			var userInboxDto = new UserInboxDto
			{
				Projects = inboxProjects,
				Messages = inboxMessagesDto
			};

			return userInboxDto;
		}

		public async Task<InboxMessageDto> GetInboxMessageByIdAsync(Guid id, int userId)
		{
			var inboxMessages = this.ClientDbContext.GetUserInboxSpResult
			.FromSqlRaw($"EXEC spCDGetUserInbox '{userId}'")
			.AsEnumerable()
			.FirstOrDefault(e=> e.Id == id) ?? throw new Exception("Message not found");

			var inboxMessageDto = mapper.Map<InboxMessageDto>(inboxMessages);
			var attachments = await ClientDbContext.EmailAttachments.Where(a => a.EmailId == inboxMessageDto.Id).ToListAsync();
			inboxMessageDto.Attachments = mapper.Map<List<EmailAttachmentDto>>(attachments);
			return inboxMessageDto;
		}

		public async Task MarkAsReadAsync(string messageId) { 
			var email = await ClientDbContext.Emails.FirstOrDefaultAsync(e => e.MessageId == messageId);
			if (email != null && !email.IsRead)
			{
				email.IsRead = true;
				await ClientDbContext.SaveChangesAsync();
			}
		}

		public async Task ArchiveAsync(string messageId)
		{
			var email = await ClientDbContext.Emails.FirstOrDefaultAsync(e => e.MessageId == messageId);
			if (email != null)
			{
				email.IsArchived = true;
				await ClientDbContext.SaveChangesAsync();
			}
		}

		public override async Task DeleteAsync(object messageId)
		{
			var email = await ClientDbContext.Emails.FirstOrDefaultAsync(e => e.MessageId == messageId);
			if (email != null)
			{
				email.IsDeleted = true;
				await ClientDbContext.SaveChangesAsync();
			}
		}
	}
}
