using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Hubs;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Net.Mime;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class ActionItemsController : BaseApiController
	{
		private readonly IActionItemsService actionItemsService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IPostMarkEmailService emailService;
		private readonly INotificationService notificationService;
		private readonly IProposalService proposalService;
		private readonly IProjectsService projectsService;
		private readonly IPDFService pDFService;
		private readonly ICompanySettingService companySettingService;
		private readonly IChangeOrderService changeOrderService;
		private readonly IRevisionsService revisionsService;
		private readonly IScheduleService scheduleService;
		private readonly ILogger<ActionItemsController> logger;
		private readonly IHubContext<ContractorDeskHub> hubContext;

		public ActionItemsController(
			IHubContext<ContractorDeskHub> hubContext,
			IActionItemsService actionItemsService,
			INotificationService notificationService,
			IPostMarkEmailService emailService,
			IProjectsService projectsService,
			IProposalService proposalService,
			IPDFService pDFService,
			IClientDocumentService clientDocumentService,
			IPermissionsService permissionsService,
			ICompanySettingService companySettingService,
			IChangeOrderService changeOrderService,
			IRevisionsService revisionsService,
			IScheduleService scheduleService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor, 
			IAzureStorageService azureStorageService,
			ILogger<ActionItemsController> logger) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService)
		{
			this.hubContext = hubContext;
			this.emailService = emailService;
			this.actionItemsService = actionItemsService;
			this.clientDocumentService = clientDocumentService;
			this.notificationService = notificationService;
			this.proposalService = proposalService;
			this.projectsService = projectsService;
			this.pDFService = pDFService;
			this.companySettingService = companySettingService;
			this.changeOrderService = changeOrderService;
			this.scheduleService = scheduleService;
			this.revisionsService = revisionsService;
			this.logger = logger;

			this.actionItemsService.UserId = this.UserId;
			this.emailService.SenderId = this.UserId;
			this.changeOrderService.UserId = this.UserId;
		}

		#region GET
		[HttpGet("action-items/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItem(int id)
		{
			var response = await actionItemsService.GetActionItemAsync(id);
			return Ok(ApiResponse<ActionItemDto>.SuccessResponse(response));
		}

		[HttpGet("action-items")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItems([FromQuery] int? d = null)
		{
			if(d != null)
			{
				var response = await actionItemsService.GetAllActionItemsCreatedInLastDaysAsync(d.Value);
				return Ok(ApiResponse<List<ActionItemDto>>.SuccessResponse(response));
			}
			else
			{
				int? supervisorId = Permissions.CanManageAllActionItems ? null : this.UserId;
				var response = await actionItemsService.GetActionItemsAsync(supervisorId);
				return Ok(ApiResponse<List<ActionItemDto>>.SuccessResponse(response));

			}
		}

		[HttpGet("action-items/project/{projectId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItemByProject(Guid projectId, [FromQuery] int? userId = null)
		{
			var response = await actionItemsService.GetByProjectIdAsync(projectId, userId);
			return Ok(ApiResponse<List<ActionItemSummaryViewDto>>.SuccessResponse(response));
		}

		[AllowAnonymous]
		[HttpGet("action-types")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionTypes()
		{
			var response = await actionItemsService.GetActionTypesAsync();
			return Ok(ApiResponse<List<ActionTypeDto>>.SuccessResponse(response));
		}

		[HttpGet("action-item/summary/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItemSummaryById(int id)
		{
			var response = await actionItemsService.GetActionItemSummaryViewByIdAsync(id);
			return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(response));
		}
		[HttpGet("action-item/{id}/details")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItemDetailsById(int id)
		{
			var response = await actionItemsService.GetActionItemShortDetailsAsync(id);
			return Ok(ApiResponse<ActionItemShortDetailsDto>.SuccessResponse(response));
		}

		[HttpGet("action-items/archived")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetArchivedActionItems()
		{
			var response = await actionItemsService.GetDashboardActionItemsAsync(Permissions.CanManageAllActionItems, this.UserId, (int)ActionItemStatus.Archived);
			return Ok(ApiResponse<List<ActionItemSummaryViewDto>>.SuccessResponse(response));
		}
		#endregion

		#region POST

		[HttpPost("action-items")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> SaveActionItem([FromBody] ActionItemPayload actionItem)
		{
			if (actionItem.Id == 0)
			{
				var actionItemDto = await CreateActionItem(actionItem);

				if(actionItemDto.AssignedSupervisors.Select(s=> s.Id).Contains(this.UserId))
				{
					var actionItemId = actionItemDto.Id;
					var acceptResult = await actionItemsService.AcceptActionItemAsync(actionItemId);
				}
				else
				{
					await SendNewActionItemNotification(actionItemDto);
				}

				await BroadcastNewActionItemToClient(actionItemDto);

				return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(actionItemDto));
			}
			else
			{
				var result = await actionItemsService.UpdateActionItemAsync(actionItem.Id, actionItem);

				if (actionItem.StartOnSaveChanges)
				{
					return await AcceptActionItem(actionItem.Id);
				}

				return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(result));
			}
		}

		[HttpPost("action-items/bulk")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> CreateActionItem([FromBody] List<ActionItemPayload> actionItems)
		{
			var createdactionItem = new List<ActionItemSummaryViewDto>();
			foreach (var actionItem in actionItems)
			{
				var item = await CreateActionItem(actionItem);
				await SendNewActionItemNotification(item);
			}

			return Ok(ApiResponse<List<ActionItemSummaryViewDto>>.SuccessResponse(createdactionItem));
		}

        [HttpPost("action-items/note")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CreateNote([FromBody] ActionItemNotePayload actionItem)
		{
			//actionItem.Title = $"Note from {CurrentUser.FirstName} {CurrentUser.LastName}";

			var actionItemDto = await actionItemsService.CreateNote(actionItem);
          
            await SendNewNoteNotification(actionItemDto);

			await BroadcastNewActionItemToClient(actionItemDto);

			return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(actionItemDto));
		}

		[HttpPost("action-items/comment")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> AddComment([FromBody] ActionItemCommentPayload payload)
		{
			var response = await actionItemsService.AddCommentAsync(payload);

			await SendNewCommentNotification(response);

			return Ok(ApiResponse<ActionItemCommentDto>.SuccessResponse(response));
		}

		#endregion

		#region PATCH

		[HttpPatch("action-items/{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdateActionItemDetails(int id, [FromBody] ActionItemDetailsUpdatePayload payload)
		{
			var result = await actionItemsService.UpdateActionItemDetails(payload);
			return Ok(ApiResponse<ActionItemShortDetailsDto>.SuccessResponse(result));
		}

		[HttpPatch("action-items/{id}/accept")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> AcceptActionItem(int id)
		{
			var result = await actionItemsService.AcceptActionItemAsync(id);
			var rootUrl = GetRootUrl();
			var user = CurrentUser;

			//BackgroundJob.Enqueue(() => this.ProcessForApproval(id, rootUrl, user));
			await SendAcceptedActionItemNotification(result.CreatedById, user);

			return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(result));
		}

		[HttpPatch("action-items/{id}/complete")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> CompleteActionItem(int id)
		{
			var result = await actionItemsService.CompleteActionItemAsync(id);

			return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(result));
		}

		[HttpPatch("action-items/{id}/archive")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> ArchiveActionItem(int id)
		{
			var actionItemDto = await ArchiveActionItem(id, true);
			return Ok(ApiResponse<ActionItemDto>.SuccessResponse(actionItemDto));
		}

		[HttpPatch("action-items/{id}/unarchive")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UnarchiveActionItem(int id)
		{
			var actionItemDto = await ArchiveActionItem(id, false);
			return Ok(ApiResponse<ActionItemDto>.SuccessResponse(actionItemDto));
		}

		[HttpPatch("action-item/update/{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdateActionItem(int id, [FromBody] ActionItemPayload payload)
		{
			if (payload == null)
				return BadRequest("Invalid payload.");

			var result = await actionItemsService.UpdateActionItemAsync(id, payload);


			return Ok(ApiResponse<ActionItemSummaryViewDto>.SuccessResponse(result));
		}

		[HttpPatch("action-item/update/supervisors/{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UpdateActionItemSupervisors(int id, [FromBody] ActionItemPayload payload)
		{
			if (payload == null)
				return BadRequest("Invalid payload.");

			var updated = await actionItemsService.UpdateActionItemSupervisors(id, payload);

			if (!updated)
				return NotFound($"ActionItem with ID {id} was not found.");

			return Ok(ApiResponse<bool>.SuccessResponse(updated));
		}

		#endregion

		#region DELETE

		[HttpDelete("action-items/{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> DeleteActionItem(int id)
		{
			var result = await actionItemsService.DeleteActionItemAsync(id);
			return Ok(ApiResponse<bool>.SuccessResponse(result));
		}

		#endregion

		#region Private Methods

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<ActionItemSummaryViewDto> CreateActionItem(ActionItemPayload actionItem)
			=> await actionItemsService.CreateAsync<ActionItemSummaryViewDto>(actionItem);


		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<ActionItemDto> ArchiveActionItem(int id, bool toArchive = false)
			=> await actionItemsService.ArchiveActionItemAsync(id, toArchive);

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
		private async Task BroadcastNewActionItemToClient(ActionItemSummaryViewDto actionItemDto)
		{
			//send the newly created action item to the creator for page auto-refresh
			await hubContext.Clients.User(this.UserId.ToString()).SendAsync(ClientNotificationType.OnActionItemCreated.GetStringValue(), actionItemDto);

			foreach (var supervisor in actionItemDto.AssignedSupervisors)
			{
				if (this.UserId != supervisor.Id)
				{
					//send the newly created action item to the creator for page auto-refresh
					await hubContext.Clients.User(supervisor.Id.ToString()).SendAsync(ClientNotificationType.OnActionItemCreated.GetStringValue(), actionItemDto);
				}
			}

		}

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendNewNoteNotification(ActionItemSummaryViewDto actionItemDto)
		{
			var userIds = new List<int> { actionItemDto.SupervisorId };
			var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
			var relatedUrl = await GetActionItemUrl(actionItemDto.Id);

			var actionItemCreatedBy = this.CurrentUser;
			string createdBy = $"{actionItemCreatedBy.FirstName} {actionItemCreatedBy.LastName}";
			var notificationMessage = $"<strong>{createdBy}</strong> created a note for you.";

			var description = $"<p>{actionItemDto.Title}<p>";
				description += $"<p>{actionItemDto.Description}<p>";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage,description, 1, relatedUrl: relatedUrl, userIds: userIds);
			await notificationService.AddNewNotification(notificationRequest);
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendNewCommentNotification(ActionItemCommentDto actionItemCommentDto)
		{
			// send to the creator of the action item
			var userId = this.UserId;
			var actionItemDto = await actionItemsService.GetActionItemShortDetailsAsync(actionItemCommentDto.ActionItemId, false);
			var userIds = new List<int>();

			userIds.AddRange(actionItemDto.AssignedSupervisors.Select(x => x.Id).ToList());
			userIds.Add(actionItemDto.CreatedByUser.Id);

			userIds = userIds.Where(u=> u != userId).ToList();

			var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
			var relatedUrl = await GetActionItemUrl(actionItemCommentDto.ActionItemId);

			var actionItemCreatedBy = this.CurrentUser;
			string createdBy = $"{actionItemCreatedBy.FirstName} {actionItemCreatedBy.LastName}";
			var notificationMessage = $"<strong>{createdBy}</strong> posted a comment on an action item";

			var description = $"<p>{actionItemCommentDto.Comment}<p>";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, description, 1, relatedUrl: relatedUrl, userIds: userIds);
			await notificationService.AddNewNotification(notificationRequest);
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

		[AutomaticRetry(Attempts = 0)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[NonAction]
		public async Task ProcessForApproval(int actionItemId, string rootUrl, ApplicationUserModel user)
		{
			//return;
			try
			{
				var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(actionItemId);
				var projectDetails = await projectsService.GetProjectShortDetailsAsync(actionItem.ProjectId.Value);
				var sysFolder = SysFolders.ChangeOrders;

				var changeOrder = await changeOrderService.CreateChangeOrderAsync(new ChangeOrderDto
				{
					ActionItemId = actionItem.Id,
					CostChangeName = actionItem.CostChangeItem,
					CurrentAmount = actionItem.CurrentAmount,
					Amount = actionItem.Amount,
					NewAmount = actionItem.CurrentAmount + actionItem.Amount,
					ScheduleChangeItem = actionItem.ScheduleChangeItem,
					NoOfDays = actionItem.NoOfDays
				});

				var clientDocument = await UploadChangeOrderDocument(actionItem, projectDetails, sysFolder, rootUrl);

				await SendChangeOrderApprovalEmail(actionItem, projectDetails, clientDocument, user, rootUrl);
				await this.clientDocumentService.SaveClientDocument(projectDetails.Id, "pdf", clientDocument.BaseFileName, clientDocument.Url, clientDocument.Version, sysFolder);
				
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error processing change order approval. for action item {actionItemId}: {ex.Message}", actionItemId);
			}
		}

		[AutomaticRetry(Attempts = 0)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[NonAction]
		public async Task ProcessCostRevisionAcknowledgement(int actionItemId, string rootUrl, ApplicationUserModel user)
		{
			try
			{
				var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(actionItemId);
				var projectDetails = await projectsService.GetProjectShortDetailsAsync(actionItem.ProjectId.Value);
				var sysFolder = SysFolders.CostRevisions;

				var costRevisionDto = await revisionsService.CreateCostRevisionAsync(new CostRevisionPayload
				{
					ActionItemId = actionItemId,
					Amount = actionItem.Amount.Value,
					EstimateCategoryId = actionItem.CostChangeItemId.Value,
					ProjectId = projectDetails.Id
				});

				var clientDocument = await UploadCostRevisionDocument(costRevisionDto, projectDetails, sysFolder, rootUrl);

				await SendCostRevisionAcknowledgementEmail(actionItem, projectDetails, clientDocument, user, rootUrl);
				await this.clientDocumentService.SaveClientDocument(projectDetails.Id, "pdf", clientDocument.BaseFileName, clientDocument.Url, clientDocument.Version, sysFolder);
				await SendAcceptedActionItemNotification(actionItem.CreatedById, user);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error processing change order approval. for action item {actionItemId}: {ex.Message}", actionItemId);
			}
		}

		[AutomaticRetry(Attempts = 0)]
		[ApiExplorerSettings(IgnoreApi = true)]
		[NonAction]
		public async Task ProcessScheduleRevisionAcknowledgement(int actionItemId, string rootUrl, ApplicationUserModel user)
		{
			try
			{
				//var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(actionItemId);
				//var projectDetails = await projectsService.GetProjectShortDetailsAsync(actionItem.ProjectId.Value);
				//var sysFolder = SysFolders.ScheduleRevisions;

				//var projectSchedule = await scheduleService.GetProjectScheduleAsync(actionItem.ProjectId.Value);
				//var affectedSchedule = projectSchedule.Tasks.FirstOrDefault(t => t.Id == actionItem.ScheduleChangeItemId);

				//DateOnly startDate = DateOnly.FromDateTime(affectedSchedule.StartDate.Value);
				//DateOnly newEndDate = startDate.AddDays(actionItem.NewDuration.Value);

				//var scheduleRevisionDto = await revisionsService.CreateScheduleRevisionsAsync(new ScheduleRevisionPayload
				//{
				//	ActionItemId = actionItemId,
				//	ConstructionTaskId = affectedSchedule.ConstructionTaskId, 
				//	Description = actionItem.Description, 
				//	Reason = actionItem.Title, 
				//	NewDuration = actionItem.NewDuration.Value, 
				//	NewStartDate = startDate,
				//	NewEndDate = newEndDate
				//}, actionItem.ProjectId.Value);

				//var clientDocument = await UploadScheduleRevisionDocument(scheduleRevisionDto, projectDetails, sysFolder, rootUrl);

				//await SendScheduleRevisionAcknowledgementEmail(actionItem, projectDetails, clientDocument, user, rootUrl);
				//await this.clientDocumentService.SaveClientDocument(projectDetails.Id, "pdf", clientDocument.BaseFileName, clientDocument.Url, clientDocument.Version, sysFolder);
				//await SendAcceptedActionItemNotification(actionItem.CreatedById, user);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, $"Error processing schedule revision for action item {actionItemId}: {ex.Message}", actionItemId);
			}
		}


		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendChangeOrderApprovalEmail(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, UploadClientDocumentResult clientDocument, ApplicationUserModel user, string rootUrl)
		{
			var companySettings = await companySettingService.GetCompanySettingAsync();

			//send email here
			var emailModel = new EmailPayloadModel
			{
				Subject = $"{companySettings.CompanyName} - Please approve the following change order",
				Body = await CreateChangeOrderEmailBody(actionItem,projectDetails,rootUrl,user.FirstName),
				ClientId = actionItem.ProjectId,
				To = projectDetails?.ClientEmailAddress
			};

			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			emailModel.ClientId = clientId;

			var attachment = new AttachmentModel
			{
				FileName = clientDocument.FileName,
				FileUrl = clientDocument.Url,
				FileStream = clientDocument.FileStream
			};

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.emailService.SendEmailAsync(emailModel);
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendCostRevisionAcknowledgementEmail(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, UploadClientDocumentResult clientDocument, ApplicationUserModel user, string rootUrl)
		{
			var companySettings = await companySettingService.GetCompanySettingAsync();

			//send email here
			var emailModel = new EmailPayloadModel
			{
				Subject = $"{companySettings.CompanyName} - Notification of a change to your construction cost",
				Body = await CreateCostRevisionEmailBody(actionItem, projectDetails, rootUrl, user.FirstName),
				ClientId = actionItem.ProjectId,
				To = projectDetails?.ClientEmailAddress
			};

			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			emailModel.ClientId = clientId;

			var attachment = new AttachmentModel
			{
				FileName = clientDocument.FileName,
				FileUrl = clientDocument.Url,
				FileStream = clientDocument.FileStream
			};

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.emailService.SendEmailAsync(emailModel);
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendScheduleRevisionAcknowledgementEmail(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, UploadClientDocumentResult clientDocument, ApplicationUserModel user, string rootUrl)
		{
			var companySettings = await companySettingService.GetCompanySettingAsync();

			//send email here
			var emailModel = new EmailPayloadModel
			{
				Subject = $"{companySettings.CompanyName} - Notification of a change to your construction schedule",
				Body = await CreateScheduleRevisionEmailBody(actionItem, projectDetails, rootUrl, user.FirstName),
				ClientId = actionItem.ProjectId,
				To = projectDetails?.ClientEmailAddress
			};

			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			emailModel.ClientId = clientId;

			var attachment = new AttachmentModel
			{
				FileName = clientDocument.FileName,
				FileUrl = clientDocument.Url,
				FileStream = clientDocument.FileStream
			};

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.emailService.SendEmailAsync(emailModel);
		}


		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<UploadClientDocumentResult> UploadChangeOrderDocument(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, SysFolders sysFolder, string rootUrl)
		{
			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			var fileContextName = actionItem.ActionTypeId switch
			{
				(int)ActionTypes.CostChange => "cost-change-order",
				(int)ActionTypes.ScheduleChange => "schedule-change-order",
				_ => "general-change-order"
			};

			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;
			var currentDate = DateTime.UtcNow.ToString("MM-dd-yyyy");
			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.{currentDate}.pdf";

			var attachmentUrl = $"{rootUrl}/pdf/change-order/{actionItem.Id}";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			return new UploadClientDocumentResult
			{
				BaseFileName = baseFileName,
				FileName = fileName,
				Url = fileUrl,
				Version = version,
				FileStream = pdfStream
			};
		}
		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<UploadClientDocumentResult> UploadCostRevisionDocument(CostRevisionDto costRevisionDto, ProjectDetailsDto projectDetails, SysFolders sysFolder, string rootUrl)
		{
			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			var fileContextName = "cost-revision";

			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;
			var currentDate = DateTime.UtcNow.ToString("MM-dd-yyyy");
			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.{currentDate}.pdf";

			var attachmentUrl = $"{rootUrl}/pdf/cost-revision/{costRevisionDto.Id}";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			return new UploadClientDocumentResult
			{
				BaseFileName = baseFileName,
				FileName = fileName,
				Url = fileUrl,
				Version = version,
				FileStream = pdfStream
			};
		}
		
		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<UploadClientDocumentResult> UploadScheduleRevisionDocument(ScheduleRevisionDto scheduleRevisionDto, ProjectDetailsDto projectDetails, SysFolders sysFolder, string rootUrl)
		{
			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			var fileContextName = "schedule-revision";

			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;
			var currentDate = DateTime.UtcNow.ToString("MM-dd-yyyy");
			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.{currentDate}.pdf";

			var attachmentUrl = $"{rootUrl}/pdf/schedule-revision/{scheduleRevisionDto.Id}";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			return new UploadClientDocumentResult
			{
				BaseFileName = baseFileName,
				FileName = fileName,
				Url = fileUrl,
				Version = version,
				FileStream = pdfStream
			};
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<String> CreateChangeOrderEmailBody(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, string rootUrl, string senderName)
		{
			var clientName = projectDetails.ClientName;
			return await Task.Run(() =>
			{
				var linkUrl = $"{rootUrl}/approvals/cost-change/{actionItem.Id}";
				var newAmount = actionItem.CurrentAmount + actionItem.Amount;
				var actionType = actionItem.ActionTypeId;

				var emailBody = $@"<p>Dear {clientName},</span></p>
							<p><span>Please approve the following Change Order:</span></p>";

				if (actionType == (int)ActionTypes.CostChange)
				{
					emailBody += $@"<p><span>Cost Change Item: {actionItem.CostChangeItem}</span><br>
							<span>Current Amount: ${actionItem.CurrentAmount}</span><br>
							<span>New Amount: ${newAmount}</span></p>";
				}
				else if (actionType == (int)ActionTypes.ScheduleChange)
				{
					emailBody += $@"<p><span>Schedule Item: {actionItem.ScheduleChangeItem}</span><br>
							<span>No. Of Days: {actionItem.NoOfDays}</span></p>";
				}
				else
				{
					emailBody += $@"<p><span>Cost Change Item: {actionItem.CostChangeItem}</span><br>
							<span>Current Amount: ${actionItem.CurrentAmount}</span><br>
							<span>New Amount: ${newAmount}</span></p>";
					emailBody += $@"<p><span>Schedule Item: {actionItem.ScheduleChangeItem}</span><br>
							<span>No. Of Days: {actionItem.NoOfDays}</span></p>";
				}		
						
				emailBody += @$"<p><a href=""{linkUrl}"" rel=""noopener"" style=""width:100px;text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">Approve</a></p>
				<p><span>Attached you will find the change order form for your records.</span></p>
				<p><br><span>Thank You,</span><br><span>{senderName}</span></p>";

				return emailBody;
			});
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<String> CreateCostRevisionEmailBody(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, string rootUrl, string senderName)
		{
			var clientName = projectDetails.ClientName;
			return await Task.Run(() =>
			{
				var linkUrl = $"{rootUrl}/approvals/revisions/{actionItem.Id}";
			
				var emailBody = $@"<p>Dear {clientName},</span></p>
							<p><span>Please ackowledge the following Cost Revision. Attached you will find a copy of this notification</span></p>";

				emailBody += @$"<p><a href=""{linkUrl}"" rel=""noopener"" style=""width:100px;text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">Acknowledge</a></p>
				<p><br><span>Thank You,</span><br><span>{senderName}</span></p>";

				return emailBody;
			});
		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task<String> CreateScheduleRevisionEmailBody(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, string rootUrl, string senderName)
		{
			var clientName = projectDetails.ClientName;
			return await Task.Run(() =>
			{
				var linkUrl = $"{rootUrl}/approvals/revisions/{actionItem.Id}";
				
				var emailBody = $@"<p>Dear {clientName},</span></p>
							<p><span>Please acknowledge the following Schedule Revision. Attached you will find a copy of this notification</span></p>";

				emailBody += @$"<p><a href=""{linkUrl}"" rel=""noopener"" style=""width:100px;text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">Acknowledge</a></p>
				<p><br><span>Thank You,</span><br><span>{senderName}</span></p>";

				return emailBody;
			});
		}


		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		private async Task SendAcceptedActionItemNotification(int toUserId, ApplicationUserModel acceptedBy)
		{
			var userIds = new List<int> { toUserId};
			var notificationCategory = NotificationCategory.ActionItem.GetStringValue();
			var relatedUrl = string.Empty; //await GetActionItemUrl(actionItemDto.Id);

			string notificationMessage = $"<strong>{acceptedBy.FirstName}</strong> accepted an Action Item.";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, string.Empty, 1, relatedUrl: relatedUrl, userIds: userIds);

			await notificationService.AddNewNotification(notificationRequest);
		}

		#endregion
	}
}
