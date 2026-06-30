using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Hubs;
using ContractorsDesk.Services.Interfaces;
using DocuSign.eSign.Model;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[AllowAnonymous]
	[Produces(MediaTypeNames.Application.Json)]
	//[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class WebhooksController : BaseApiController
	{
		private readonly ITenantService tenantService;
		private readonly IProjectsService projectsService;
		private readonly IAudioUploadsService audioUploadsService;
        private readonly INotificationService notificationService;
		private readonly IActionItemsService actionItemsService;
		private readonly IProjectJournalService projectJournalService;
		private readonly IEmailsService emailsService;
		private readonly IUserLogService userLogService;
		private readonly IHubContext<ContractorDeskHub> hubContext;

		public WebhooksController(
			ITenantService tenantService,
			IProjectsService projectsService,
			IActionItemsService actionItemsService,
			IAudioUploadsService audioUploadsService,
			IEmailsService emailsService,
            INotificationService notificationService,
            IPermissionsService permissionsService,
			IProjectJournalService projectJournalService,
			IUserLogService userLogService,
			IHubContext<ContractorDeskHub> hubContext,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor,
			IAzureStorageService azureStorageService) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService)
		{
			this.tenantService = tenantService;
			this.projectsService = projectsService;
			this.actionItemsService = actionItemsService;
			this.audioUploadsService = audioUploadsService;
            this.notificationService = notificationService;
			this.projectJournalService = projectJournalService;
			this.emailsService = emailsService;
			this.userLogService = userLogService;
			this.hubContext = hubContext;
        }

		#region Public
		[HttpPost("webhooks/audio-upload")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UploadAudioFile([FromBody] AudioUploadResponsePayload payload)
		{
			var response = await audioUploadsService.UpdateAudioFileUploadStatusAsync(payload);
			await SendNotification(response);

            return Ok(ApiResponse<Guid>.SuccessResponse(response.Id));
		}

		[HttpPost("webhooks/audio-upload/transcript")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateAudioUploadTranscript([FromBody] AudioUploadUpdateTranscriptPayload payload)
		{
			var audioUploadObj = new AudioUploadResponsePayload();
			audioUploadObj.Ref = payload.Ref;
			audioUploadObj.Transcript = payload.Transcript;
			audioUploadObj.ProjectId = payload.ProjectId == Guid.Empty ? null : payload.ProjectId;

			var response = await audioUploadsService.UpdateAudioFileUploadStatusAsync(audioUploadObj);
			await SendNotification(response);

			return Ok(ApiResponse<Guid>.SuccessResponse(response.Id));
		}

		[HttpPost("webhooks/action-items/create")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> SaveActionItem([FromBody] ActionItemPayload actionItem)
		{
			actionItem.IsAiCreated = true;
			actionItemsService.UserId = actionItem.Supervisors?.FirstOrDefault() ?? 0;
			var result = await actionItemsService.CreateAsync<ActionItemSummaryViewDto>(actionItem);
			await SendNewActionItemNotification(result);

			return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(result));
		}

		[HttpGet("webhooks/action-items/filter")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItemsCreatedSince([FromQuery] ActionItemsCreatedSincePayload payload)
		{
			var response = await actionItemsService.GetActionItemsCreatedSinceAsync(payload.DateSince, payload.ProjectId);
			return Ok(ApiResponse<List<ActionItemDto>>.SuccessResponse(response));
		}

		[HttpGet("webhooks/project-journals/latest-datestamp")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetLatestJournalDateAsyncGet([FromQuery]Guid? projectId = null)
		{
			var response = await projectJournalService.GetLatestJournalDateAsync(projectId);
			return Ok(ApiResponse<DateTime>.SuccessResponse(response));
		}

		[HttpPost("webhooks/activity-stream")]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		public async Task<IActionResult> CreateActionItem([FromBody] ActivityStreamPayload activityStreamPayload)
		{
			// await activityStreamService.CreateActivityStream(activityStreamPayload);
			return Ok(ApiResponse<string>.SuccessResponse("Accepted"));
		}

		[HttpPost("webhooks/postmark-inbound-email")]
		public async Task<IActionResult> ReceiveInboundEmail()
		{
			using var reader = new StreamReader(Request.Body);
			var rawBody = await reader.ReadToEndAsync();

			var options = new JsonSerializerOptions
			{
				PropertyNameCaseInsensitive = true
			};

			var payload = JsonSerializer.Deserialize<PostMarkInboundEmailModel>(rawBody, options);

			//<bfce9dcc-9aa1-4fc7-9d99-12993792bed6@mtasv.net> <CAFpE4jEUWM3q1zqTiVxu-OzdQPGFNqS2K8OsFjuYErxoUBq+Sg@mail.gmail.com> <CAFpE4jFzpwCAyF=eoA4SsLNJvKdXqMREhz_Kg8YDP9v+=mrmLw@mail.gmail.com>
			//get the email id of the messaged being replied to
			var subdomain = payload.ToFull[0].Email.Split("@")[0];
			await this.SetClientDatabase(subdomain);

			var references = payload.Headers?.FirstOrDefault(h => h.Name.ToLower() == "references")?.Value;
			var postmarkMessageId = payload.Headers?.FirstOrDefault(h => h.Name.ToLower() == "message-id")?.Value;
			var messageId = references?.Split(' ').FirstOrDefault();
			if (!string.IsNullOrEmpty(messageId))
			{
				messageId = messageId.Trim('<', '>').Split('@')[0];
			}

			// get the from email address
			payload.From = payload.From?.Trim().ToLower();
			

			var payloadHtmlBody = payload.HtmlBody;
			var doc = new HtmlDocument();
			doc.LoadHtml(payloadHtmlBody);

			// Select the first <div> element
			var firstDiv = doc.DocumentNode.SelectSingleNode("//div[1]");

			// Get its full HTML
			string body = firstDiv?.OuterHtml;
			// Output: <div dir="ltr">Sounds great, thanks for the update! </div>
			var attachments = new List<AttachmentModel>();


			var email = await emailsService.GetEmailByMessageId(messageId);
			string attachmentFolderName = email.ProjectId != null ? email.ProjectId.Value.ToString() : "attachments"; 

			var emailModel = new EmailModel
			{
				Subject = payload.Subject,
				Body = body,
				From = payload.From,
				To = payload.To,
				Cc = payload.Cc,
				Bcc = payload.Bcc,
				MessageId = payload.MessageID,
				ReplyToMessageId = messageId,
				PostMarkReferences = references,
				IsRead = false,
				EmailType = (int?)EmailTypes.ClientEmail
			};

			if (payload.Attachments != null && payload.Attachments.Any())
			{
				foreach (var attachment in payload.Attachments)
				{
					byte[] bytes = Convert.FromBase64String(attachment.Content);
					var stream = new MemoryStream(bytes);

					var fileUrl = await this.azureStorageService.UploadFileFromStream(stream, "client-documents", attachment.Name, $"{attachmentFolderName}/{SysFolders.ClientEmails.GetStringValue()}");

					// Here, you would typically save the attachment to your storage and get a URL
					// For demonstration, we'll just create a placeholder URL
					var attachmentModel = new AttachmentModel
					{
						FileName = attachment.Name,
						FileUrl = fileUrl // Placeholder URL
					};
					attachments.Add(attachmentModel);
				}
				emailModel.Attachments = attachments;
			}

			var emailId = await emailsService.SaveEmailAsync(emailModel);

			if(email != null)
			{
				var userIds = new List<int> { email.SenderId.Value };
				var notificationCategory = NotificationCategory.NewEmail.GetStringValue();
				var relatedUrl = "/emails"; //await GetActionItemUrl(actionItemDto.Id);

				var inboxItem = await emailsService.GetInboxMessageByIdAsync(emailId, email.SenderId.Value);

				string notificationMessage = $"<strong>{inboxItem.ProjectName} - New Email Received:</strong><br/> " +
					$"From: {inboxItem.SenderName} <br/>" +
					$"Message: <span class='text-muted'>{inboxItem.Message}</span>";

				var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, string.Empty, 1, relatedUrl: relatedUrl, userIds: userIds, emailId: inboxItem.Id);

				await notificationService.AddNewNotification(notificationRequest);

				await hubContext.Clients.User(email.SenderId.Value.ToString()).SendAsync(ClientNotificationType.OnNewEmailReceived.GetStringValue(), inboxItem);

			}

			return Ok();
		}

		[HttpPost("webhooks/userlog")]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		public async Task<IActionResult> CreatePageLog([FromBody] UserLogPayload payload)
		{
			await userLogService.CreateLogAsync(payload.Path, this.UserId);
			return Ok(ApiResponse<string>.SuccessResponse("Accepted"));
		}

		#endregion

		#region Private
		[NonAction]
		private async Task SendNotification(AudioUpload audioUpload)
        {
            var userIds = new List<int> { audioUpload.UserId };
            var notificationCategory = NotificationCategory.AI.GetStringValue();
            var message = NotificationMessage.ProcessedByAI.GetStringValue();
            string notificationMessage = "AI has completed processing your audio.";

            var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, "", 1, userIds: userIds);

            await notificationService.AddNewNotification(notificationRequest);
        }

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendNewActionItemNotification(ActionItemSummaryViewDto actionItemDto)
		{
			var userIds = actionItemDto.AssignedSupervisors?.Select(s => s.Id).ToList();
			var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
			var relatedUrl = await GetActionItemUrl(actionItemDto.Id);
			var notificationMessage = string.Empty;

			var actionItemCreatedBy = actionItemDto.CreatedBy;
			if (actionItemCreatedBy != null)
			{
				notificationMessage = $"<strong>{actionItemDto.CreatedBy}</strong> assigned an Action Item to you.";
			}
			else
			{
				notificationMessage = "An <strong>Action Item</strong> has been assigned to you.";
			}

			var description = $"<p>{actionItemDto.Title}<p>";
			description += $"<p>{actionItemDto.Description}<p>";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, description, 1, relatedUrl: relatedUrl, userIds: userIds);
			await notificationService.AddNewNotification(notificationRequest, NotificationNextActionEnum.ReloadActionItemDashboard);
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<string> GetActionItemUrl(int actionItemId)
		{
			return await Task.Run(() => {
				var request = HttpContext.Request;
				var rootUrl = $"{request.Scheme}://{request.Host}";
				var actionItemUrl = $"{rootUrl}/action-items/{actionItemId}";
				return actionItemUrl;
			});
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[NonAction]
		private async Task SetClientDatabase(string subDomain)
		{
			var clientDatabase = await tenantService.GetClientDatabase(subDomain);

			this.emailsService.ClientDbContext = clientDatabase;
			this.notificationService.ClientDbContext = clientDatabase;
			this.userLogService.ClientDbContext = clientDatabase;
			this.actionItemsService.ClientDbContext = clientDatabase;
			this.projectsService.ClientDbContext = clientDatabase;
			this.projectJournalService.ClientDbContext = clientDatabase;
		}


		#endregion
	}
}
