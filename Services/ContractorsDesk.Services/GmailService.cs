using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace ContractorsDesk.Services
{
	public class GmailService : BaseService, IGmailService
	{
		private readonly IAzureStorageService azureStorageService;
		private const string GmailSmtpServer = "smtp.gmail.com";
		private const int SmtpPort = 587;

		public GmailService(
			IAzureStorageService azureStorageService,
			ClientDbContext clientDbContext,
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDbContext, masterDbContext, configuration) 
			{ 
				this.azureStorageService = azureStorageService;
			}

		#region Public
		public int SenderId { get; set; }
		public string SenderName { get; set; }
		public string SubDomain { get; set; }
		public async Task<bool> SendEmailAsync(EmailPayloadModel emailModel)
		{
			var from = configuration["GmailSmtpServer:From"] ?? string.Empty;
			var appName = configuration["GmailSmtpServer:AppName"] ?? string.Empty;
			var appPassword = configuration["GmailSmtpServer:AppPassword"] ?? string.Empty;

			var email = await CreateEmailMessage(appName, from, emailModel);

			using (var smtpClient = new SmtpClient())
			{
				try
				{
					await smtpClient.ConnectAsync(GmailSmtpServer, SmtpPort, MailKit.Security.SecureSocketOptions.StartTls);
					await smtpClient.AuthenticateAsync(from, appPassword);
					await smtpClient.SendAsync(email);
					await smtpClient.DisconnectAsync(true);
					await this.CreateSentEmail(emailModel);

					return true;
				}
				catch (Exception) { throw; }
			}
		}

		public async Task<bool> MarkSentEmailAsRead(Guid id)
		{
			var email = await ClientDbContext.Emails
				.Where(n => n.Id == id)
				.FirstOrDefaultAsync();

			if (email == null) throw new Exception("Email not found.");

			email.IsRead = true;
			ClientDbContext.Emails.Update(email);
			await ClientDbContext.SaveChangesAsync();
			return true;
		}

        public async Task<List<SentEmailDto>> GetSentEmailsAsync()
        {
            int userId = this.UserId;

            var emails = await ClientDbContext.Emails
                .Include(e => e.EmailAttachments)
                .Where(e => e.CreatedBy == userId)
                .OrderByDescending(e => e.DateCreated)
                .ToListAsync();

            var emailDtos = emails
                .Select(email => mapper.Map<SentEmailDto>(email))
                .ToList();

            var projectIds = emails
                .Where(e => e.ProjectId.HasValue)
                .Select(e => e.ProjectId.Value)
                .Distinct()
                .ToList();

            var projectMap = await ClientDbContext.Qbclasses
                .Where(p => projectIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.Name);

            foreach (var dto in emailDtos)
            {
                var projectId = emails.First(e => e.Id == dto.Id).ProjectId;

                if (projectId.HasValue && projectMap.TryGetValue(projectId.Value, out var projectName))
                {
                    dto.ProjectName = projectName;
                }
                else
                {
                    dto.ProjectName = null;
                }
            }

            return emailDtos;
        }

        public async Task<SentEmailDto> GetSentEmailByIdAsync(Guid id)
		{
            var email = await ClientDbContext.Emails
			 .Include(n => n.EmailAttachments)
			 .FirstOrDefaultAsync(n => n.Id == id)
			 ?? throw new ArgumentException($"Unable to find email with an Id of {id}");

            var sentEmailDto = mapper.Map<SentEmailDto>(email);

            if (email.ProjectId.HasValue)
            {
                var project = await ClientDbContext.Qbclasses
                    .Where(p => p.Id == email.ProjectId.Value)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                sentEmailDto.ProjectName = project;
            }

            return sentEmailDto;
        }

        public async Task<List<string>> GetToEmailAddresses()
        {
            var toFields = await ClientDbContext.Emails
                .Where(e => !string.IsNullOrEmpty(e.To))
                .Select(e => e.To)
                .ToListAsync();

            var distinctEmails = toFields
                .SelectMany(to => to.Split(',', StringSplitOptions.RemoveEmptyEntries))
                .Select(email => email.Trim().ToLowerInvariant())
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .Distinct()
                .ToList();

            return distinctEmails;
        }
		public Task<bool> SendReplyAsync(EmailPayloadModel emailModel)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Private

		private async Task<MimeMessage> CreateEmailMessage(string appName, string from, EmailPayloadModel emailModel)
		{
			var email = new MimeMessage();
			email.From.Add(new MailboxAddress(appName, from));
			email.Subject = emailModel.Subject;

			await PopulateRecipients(email, emailModel);
			await PopulateBody(email, emailModel);

			return email;
		}
		private async Task PopulateRecipients(MimeMessage email, EmailPayloadModel emailModel)
		{
			await Task.Run(() =>
			{
				// Check if running in localhost or test environment
				var environment = (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production").ToLower();
				var isTestEnvironment = environment == "test" || environment == "development" || environment == "localhost";
				var overrideEmail = configuration["EmailSettings:EmailRecipient"];

				var recipients = isTestEnvironment ? new[] { overrideEmail } : emailModel.To.Split(',');
				email.To.AddRange(recipients.Select(to => MailboxAddress.Parse(to.ToLower())));


				if (!string.IsNullOrEmpty(emailModel.Cc) && !isTestEnvironment)
				{
					var ccs = emailModel.Cc.Split(',');
					email.Cc.AddRange(ccs.Select(cc => MailboxAddress.Parse(cc.ToLower())));
				}

				if (!string.IsNullOrEmpty(emailModel.Bcc) && !isTestEnvironment)
				{
					var bccs = emailModel.Bcc.Split(',');
					email.Bcc.AddRange(bccs.Select(bcc => MailboxAddress.Parse(bcc.ToLower())));

				}
			});
		}
		private async Task PopulateBody(MimeMessage email, EmailPayloadModel emailModel)
		{
			string emailBody = emailModel.Body;
			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = emailBody,
				TextBody = emailBody
			};

			if(emailModel.Attachments != null && emailModel.Attachments.Any())
			{
				await PopulateAttachments(email, emailModel, bodyBuilder);
			}

			email.Body = bodyBuilder.ToMessageBody();
		}
		private async Task PopulateAttachments(MimeMessage email, EmailPayloadModel emailModel, BodyBuilder bodyBuilder)
		{
			var emailSizeResult = CheckEmailSize(emailModel);
			if (emailModel.HasAttachments())
			{
				foreach (var attachment in emailModel.Attachments)
				{
					if (string.IsNullOrEmpty(attachment.FileUrl))
					{
						var pdfStream = attachment.FileStream;
						var fileName = attachment.FileName;
						var directory = attachment.Directory;

						var fileUrl = await azureStorageService.UploadFileFromStream(pdfStream, "email-attachments", fileName, directory);

						attachment.FileUrl = fileUrl;
					}
				}

				if (emailSizeResult.ExceedsLimit)
				{
					AttachFilesAsLinks(email, emailModel, bodyBuilder, emailSizeResult.TotalSize);
				}
				else
				{
					AttachFiles(email, emailModel, bodyBuilder);
				}
			}
		}
		private void AttachFiles(MimeMessage email, EmailPayloadModel emailModel, BodyBuilder bodyBuilder)
		{
			if (emailModel.HasAttachments())
			{
				foreach (var attachment in emailModel.Attachments)
				{
					var stream = attachment.FileStream;

					var emailAttachment = new MimePart()
					{
						Content = new MimeContent(attachment.FileStream),
						ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
						ContentTransferEncoding = ContentEncoding.Base64,
						FileName = attachment.FileName
					};

					bodyBuilder.Attachments.Add(emailAttachment);
				}
			}
		}
		private void AttachFilesAsLinks(MimeMessage email, EmailPayloadModel emailModel, BodyBuilder bodyBuilder, long emailSize)
		{
			// create logic here to convert attachments to links
			// set base for total attachments here for 24MB 
			if(emailModel.HasAttachments())
			{
				var maxTotalAttachmentsSizeInBytes = 24 * 1024 * 1024;
				var attachmentCount = emailModel.Attachments.Count();
				var maxSizePerAttachment = maxTotalAttachmentsSizeInBytes / attachmentCount;
				var attachmentLinks = string.Empty;

				foreach (var attachment in emailModel.Attachments)
				{
					var attachmentSize = attachment.FileStream.Length;
					if (attachmentSize > maxSizePerAttachment)
					{
						attachmentLinks += $"<a href='{attachment.FileUrl}'>{attachment.FileName}</a><br/>";
					}
					else
					{
						bodyBuilder.Attachments.Add(new MimePart()
						{
							Content = new MimeContent(attachment.FileStream),
							ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
							ContentTransferEncoding = ContentEncoding.Base64,
							FileName = attachment.FileName
						});
					}
				}

				if (attachmentLinks != string.Empty)
				{
					bodyBuilder.HtmlBody += "<br/><strong>Large File Attachments Links:</strong><br/>" + attachmentLinks;
					bodyBuilder.TextBody += "\nLarge File Attachments Links:\n" + attachmentLinks;
				}
			}
			
		}
		private async Task<SentEmailDto> CreateSentEmail(EmailPayloadModel emailModel)
		{
			int userId = this.UserId;

			var email = new DataStore.Client.Models.Email
			{
				Id = Guid.NewGuid(),
				ProjectId = emailModel.ClientId,
				Subject = emailModel.Subject ?? string.Empty,
				Body = emailModel.Body,
				Cc = emailModel.Cc,
				Bcc = emailModel.Bcc,
				CreatedBy = userId,
				DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				To = emailModel.To,
				EmailType = emailModel.Type.HasValue ? (int)emailModel.Type : null,
			};

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

			return new SentEmailDto
			{
				Id = email.Id,
				Subject = email.Subject,
				Body = email.Body,
				Cc = email.Cc,
				Bcc = email.Bcc,
				DateCreated = email.DateCreated,
				To = email.To
			};
		}
		private (bool ExceedsLimit, long TotalSize) CheckEmailSize(EmailPayloadModel emailModel)
		{
			const int maxSizeInBytes = 25 * 1024 * 1024; // 25 MB
			const double base64Overhead = 1.33;
			long totalAttachmentSize = 0;
			// Calculate email body size
			int emailBodySize = System.Text.Encoding.UTF8.GetByteCount(emailModel.Body);

			// Calculate total attachment size
			if (emailModel.Attachments != null && emailModel.Attachments.Any())
			{
				var attachments = emailModel.Attachments.Select(a => a.FileStream).ToList();
				totalAttachmentSize = attachments.Sum(a => a.Length);
			}

			// Calculate total size with Base64 overhead
			double totalSize = (emailBodySize + totalAttachmentSize) * base64Overhead;
			return (totalSize > maxSizeInBytes, (long)totalSize);
		}

	
		#endregion
	}
}
