using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.DataStore.Master.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using DocuSign.eSign.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PostmarkDotNet;
using PostmarkDotNet.Model;

namespace ContractorsDesk.Services
{
	public class PostMarkEmailService : BaseService, IPostMarkEmailService
	{
		private readonly IAzureStorageService azureStorageService;
		private readonly string APIKey;
		private readonly ITenantService tenantService;

		public PostMarkEmailService(
			ITenantService tenantService,
			ClientDbContext clientDbContext,
			MasterDbContext masterDbContext,
			IAzureStorageService azureStorageService,
			IMapper mapper,
			IConfiguration configuration)
			: base(mapper, clientDbContext, masterDbContext, configuration) { 
				this.azureStorageService = azureStorageService;
				this.APIKey = configuration["EmailSettings:APIKey"];
				this.tenantService = tenantService;
		}
		public int SenderId { get; set; }
		public string SenderName { get; set; }
		public string SubDomain { get; set; }

		#region Public
		public async Task<bool> SendEmailAsync(EmailPayloadModel emailModel)
		{
			//var apiKey = configuration["EmailSettings:APIKey"];
			var client = new PostmarkClient(this.APIKey);
			

			var senderId = this.SenderId != 0 ? this.SenderId : 1;
			var senderName = !string.IsNullOrEmpty(this.SenderName) ? this.SenderName : "ContractorsDesk Admin";

			var initials = string.Join("", senderName
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Take(2)
				.Select(word => word[0]));

			//var from = $"{senderName} <{initials.ToLower()}{senderId}@contractors-desk.com>";
			var from = senderName;

			var message = await CreateEmailMessage(emailModel);
			try
			{
				var result = await client.SendMessageAsync(message);

				if (result.Status == PostmarkStatus.Success && emailModel.SaveToDatabase)
				{
					var sentEmail = await CreateSentEmail(emailModel, from, result.MessageID);
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}

			return false;
		}

		public async Task<bool> SendReplyAsync(EmailPayloadModel emailModel)
		{
			var client = new PostmarkClient(this.APIKey);

			var senderId = this.SenderId != 0 ? this.SenderId : 1;
			var senderName = !string.IsNullOrEmpty(this.SenderName) ? this.SenderName : "ContractorsDesk Admin";

			var email = await ClientDbContext.Emails
				.Where(e => e.MessageId == emailModel.MessageId)
				.FirstOrDefaultAsync()
				?? throw new ArgumentException($"Unable to find email with a message id of {emailModel.MessageId}");

			string references = email.PostMarkReferences;

			//var from = $"{senderName} <{initials.ToLower()}{senderId}@contractors-desk.com>";
			//var replyTo = $"{this.SenderName} <d6fb0681da23e675cb2d8591b4073887@inbound.postmarkapp.com>";

			var message = await CreateEmailMessage(emailModel);

			message.Headers = new HeaderCollection
			{
				new MailHeader
				{
					Name = "In-Reply-To",
					Value = references
				},
				new MailHeader
				{
					Name = "Message-ID",
					Value = email.MessageId
				}
			};

			try
			{
				var result = await client.SendMessageAsync(message);

				if (result.Status == PostmarkStatus.Success)
				{
					await CreateSentReply(emailModel, result.MessageID, email.ReplyToMessageId);
					return true;
				}
			}
			catch (Exception ex)
			{
				return false;
			}

			return false;


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

		#endregion

		#region Private

		private async Task<PostmarkMessage> CreateEmailMessage(EmailPayloadModel emailModel)
		{
			var inboundAddress = $"{SubDomain}@contractors-desk.com?mailboxhash={SubDomain}";

			var from = $"{this.SenderName} <{SubDomain}@contractors-desk.com>";
			var email = new PostmarkMessage
			{
				From = from, 
				To = emailModel.To,
				Subject = emailModel.Subject,
				TextBody = emailModel.Body,
				HtmlBody = emailModel.Body,
				TrackOpens = true, 
				ReplyTo = inboundAddress
			};

			await PopulateRecipients(email, emailModel);
			await PopulateBody(email, emailModel);

			return email;
		}
		private async Task PopulateRecipients(PostmarkMessage email, EmailPayloadModel emailModel)
		{
			await Task.Run(() =>
			{
				// Check if running in localhost or test environment
				var environment = (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production").ToLower();
				var isTestEnvironment = environment == "test" || environment == "development" || environment == "localhost";
				var overrideEmail = configuration["EmailSettings:EmailRecipient"];

				//var recipients = isTestEnvironment ? new[] { overrideEmail } : emailModel.To.Split(',');
				var recipients = isTestEnvironment ? overrideEmail  : emailModel.To;
				email.To = recipients;


				if (!string.IsNullOrEmpty(emailModel.Cc) && !isTestEnvironment)
				{
					email.Cc = emailModel.Cc;
				}

				if (!string.IsNullOrEmpty(emailModel.Bcc) && !isTestEnvironment)
				{
					email.Bcc = emailModel.Bcc;

				}
			});
		}
		private async Task PopulateBody(PostmarkMessage email, EmailPayloadModel emailModel)
		{
			string emailBody = emailModel.Body;

			email.HtmlBody = emailBody;

			if (emailModel.Attachments != null && emailModel.Attachments.Any())
			{
				await PopulateAttachments(email, emailModel, emailBody);
			}
		}
		private async Task PopulateAttachments(PostmarkMessage email, EmailPayloadModel emailModel, string bodyBuilder)
		{
			email.Attachments = new List<PostmarkMessageAttachment>();
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
					await AttachFilesAsLinks(email, emailModel, bodyBuilder, emailSizeResult.TotalSize);
				}
				else
				{
					await AttachFiles(email, emailModel, bodyBuilder);
				}
			}
		}
		private async Task AttachFiles(PostmarkMessage email, EmailPayloadModel emailModel, string bodyBuilder)
		{
			await Task.Run(async () =>
			{
				if (emailModel.HasAttachments())
				{
					foreach (var attachment in emailModel.Attachments)
					{
						var emailAttachment = await CreateAttachment(attachment);
						email.Attachments.Add(emailAttachment);
					}
				}
			});
			
		}
		private async Task<PostmarkMessageAttachment> CreateAttachment(AttachmentModel attachment)
		{
			return await Task.Run(() =>
			{
				var stream = attachment.FileStream;

				// Reset stream position if needed
				if (stream.CanSeek)
					stream.Seek(0, SeekOrigin.Begin);

				using var memoryStream = new MemoryStream();
				stream.CopyTo(memoryStream);
				byte[] attachmentBytes = memoryStream.ToArray();

				var emailAttachment = new PostmarkMessageAttachment
				{
					Content = Convert.ToBase64String(attachmentBytes),
					Name = attachment.FileName,
					ContentType = "application/octet-stream" // You can set the appropriate content type based on the file type
				};

				return emailAttachment;
			});
			
		}
		private async Task AttachFilesAsLinks(PostmarkMessage email, EmailPayloadModel emailModel, string bodyBuilder, long emailSize)
		{
			await Task.Run(async () =>
			{
				// create logic here to convert attachments to links
				// set base for total attachments here for 10MB (POSTMARK LIMIT) 
				if (emailModel.HasAttachments())
				{
					var maxTotalAttachmentsSizeInBytes = 10485760;//24 * 1024 * 1024;
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
							var emailAttachment = await CreateAttachment(attachment);
							email.Attachments.Add(emailAttachment);
						}
					}

					if (attachmentLinks != string.Empty)
					{
						bodyBuilder += "<br/><strong>Large File Attachments Links:</strong><br/>" + attachmentLinks;
					}

					email.HtmlBody = bodyBuilder;
					email.TextBody = bodyBuilder;
				}
			});
			
		}
		private async Task<SentEmailDto> CreateSentEmail(EmailPayloadModel emailModel, string from, Guid messageId)
		{
			//this.ClientDbContext = await this.tenantService.GetClientDatabase(this.SubDomain);

			int userId = this.UserId;
			var email = new DataStore.Client.Models.Email
			{
				Id = Guid.NewGuid(),
				ProjectId = emailModel.ClientId,
				SenderId = this.SenderId != 0 ? this.SenderId : 1,
				Subject = emailModel.Subject ?? string.Empty,
				From = from,
				To = emailModel.To,
				Body = emailModel.Body,
				MessageId = messageId.ToString(),
				Cc = emailModel.Cc,
				Bcc = emailModel.Bcc,
				CreatedBy = userId,
				DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
				EmailType = emailModel.Type.HasValue ? (int)emailModel.Type : null,
			};

			this.ClientDbContext.Emails.Add(email);


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

					this.ClientDbContext.EmailAttachments.Add(emailAttachment);
				}
				
			}

			await this.ClientDbContext.SaveChangesAsync();

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
		private async Task CreateSentReply(EmailPayloadModel emailModel, Guid messageId, string replyToMessageId)
		{
			int userId = this.UserId;
			var from = $"{this.SenderName} <user@contractors-desk.com>";

			var email = new DataStore.Client.Models.Email
			{
				Id = Guid.NewGuid(),
				ProjectId = emailModel.ClientId,
				SenderId = this.SenderId != 0 ? this.SenderId : 1,
				Subject = emailModel.Subject ?? string.Empty,
				ReplyToMessageId = replyToMessageId,
				From = from,
				To = emailModel.To,
				Body = emailModel.Body,
				MessageId = messageId.ToString(),
				Cc = emailModel.Cc,
				Bcc = emailModel.Bcc,
				CreatedBy = userId,
				IsRead = true,
				DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
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
		}
		private (bool ExceedsLimit, long TotalSize) CheckEmailSize(EmailPayloadModel emailModel)
		{
			const int maxSizeInBytes = 10485760; //10MB //25 * 1024 * 1024; // 25 MB
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
