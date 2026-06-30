using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[AllowAnonymous]
	[Route("approvals")]
	public class ApprovalsController : Controller
	{
		private readonly IActionItemsService actionItemsService;
		private readonly INotificationService notificationService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IPostMarkEmailService emailService;
		private readonly IAzureStorageService azureStorageService;
		private readonly IPDFService pDFService;
		private readonly ICompanySettingService companySettingService;
		private readonly IProjectsService projectsService;
		private readonly IRevisionsService revisionsService;
		public ApprovalsController(
			IActionItemsService actionItemsService, 
			INotificationService notificationService,
			IClientDocumentService clientDocumentService,
			ICompanySettingService companySettingService,
			IProjectsService projectsService,
			IAzureStorageService azureStorageService,
			IRevisionsService revisionsService,
			IPostMarkEmailService emailService,
			IPDFService pDFService)
		{
			this.actionItemsService = actionItemsService;
			this.notificationService = notificationService;
			this.clientDocumentService = clientDocumentService;
			this.pDFService = pDFService;
			this.azureStorageService = azureStorageService;
			this.emailService = emailService;
			this.companySettingService = companySettingService;
			this.projectsService = projectsService;
			this.revisionsService = revisionsService;
        }

		#region Public
		
		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("cost-change/{id}")]
		public async Task<IActionResult> CostChange(int id)
		{
			//get action item
			var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(id);
			if(actionItem.StatusId == (int)ActionItemStatus.ClientApproved) return View(new { Expired = true });

			//process action item approval here
			var rootUrl = GetRootUrl();
			var result = await actionItemsService.ApproveActionItemAsync(actionItem);
			BackgroundJob.Enqueue(() => ProcessChangeOrderDocumentsAsync(result, rootUrl));

			return View();
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("revisions/{id}")]
		public async Task<IActionResult> Revisions(int id)
		{
			//get action item
			var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(id);
			if (actionItem.StatusId == (int)ActionItemStatus.ClientApproved) return View(new { Expired = true });

			//process action item approval here
			var rootUrl = GetRootUrl();
			var result = await actionItemsService.AcknowledgeActionItemAsync(actionItem);

			BackgroundJob.Enqueue(() => ProcessRevisionDocumentsAsync(result, rootUrl));

			return View();
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("schedule-revisions/{id}")]
		public async Task<IActionResult> ScheduleRevisions(Guid id)
		{
			var scheduleRevision = await revisionsService.GetScheduleRevisionByIdAsync(id);
			await actionItemsService.SetActionItemStatusAsync(scheduleRevision.ActionItemId.Value, ActionItemStatus.ClientAcknowledged);
			return View();
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		[AllowAnonymous]
		[HttpGet("cost-revisions/{id}")]
		public async Task<IActionResult> CostRevisions(Guid id)
		{
			var costRevisionDto = await revisionsService.GetCostRevisionAsync(id);
			await actionItemsService.SetActionItemStatusAsync(costRevisionDto.ActionItemId.Value, ActionItemStatus.ClientAcknowledged);
			return View();
		}


		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		[AutomaticRetry(Attempts = 0)]
		public async Task ProcessRevisionDocumentsAsync(ActionItemSummaryViewDto actionItem, string rootUrl)
		{
			var companySettings = await companySettingService.GetCompanySettingAsync();
			var projectDetails = await projectsService.GetProjectShortDetailsAsync(actionItem.ProjectId.Value);
			var clientName = projectDetails.ClientName;
			var clientId = projectDetails.Id;
			var revisionType = actionItem.ActionTypeId == (int)ActionTypes.CostChange ? "Cost Revision" : "Schedule Revision";
			var reportName = actionItem.ActionTypeId == (int)ActionTypes.CostChange ? "Estimate To Actual Report" : "Schedule Report";

			var emailBody = $@"<p>Dear {clientName},&nbsp;</p><p>Thank you for acknowledging the {revisionType}. Attached is a copy of your {reportName}.&nbsp;</p>
								<p>Please review and let's meet or call to discuss.&nbsp;</p><p>&nbsp;</p><p>
								Thank you,&nbsp;</p><p>{companySettings.CompanyName}</p>";

			var emailModel = new EmailPayloadModel
			{
				Subject = $"{companySettings.CompanyName} - Here is the {revisionType} Acknowledgement",
				Body = emailBody,
				ClientId = actionItem.ProjectId,
				To = projectDetails.ClientEmailAddress
			};


			await ProcessAttachments(actionItem, projectDetails, emailModel, rootUrl);

			var emailSent = await this.emailService.SendEmailAsync(emailModel);

		}

		[NonAction]
		[ApiExplorerSettings(IgnoreApi = true)]
		[AutomaticRetry(Attempts = 0)]
		public async Task ProcessChangeOrderDocumentsAsync(ActionItemSummaryViewDto actionItem, string rootUrl)
		{
			var companySettings = await companySettingService.GetCompanySettingAsync();
			var projectDetails = await projectsService.GetProjectShortDetailsAsync(actionItem.ProjectId.Value);
			var clientName = projectDetails.ClientName;
			var clientId = projectDetails.Id;


			var emailBody = $@"<p>Dear {clientName},&nbsp;</p><p>Thank you for approving the Change Order Request. Attached is a copy of your Estimate To Actual Report.&nbsp;</p>
								<p>Please review and let's meet or call to discuss.&nbsp;</p><p>&nbsp;</p><p>
								Thank you,&nbsp;</p><p>CH Anderson Construction</p>";

			var emailModel = new EmailPayloadModel
			{
				Subject = $"{companySettings.CompanyName} - Here is the Change Order Confirmation",
				Body = emailBody,
				ClientId = actionItem.ProjectId,
				To = projectDetails.ClientEmailAddress
			};


			await ProcessAttachments(actionItem, projectDetails, emailModel, rootUrl);

			var emailSent = await this.emailService.SendEmailAsync(emailModel);

		}


		#endregion

		#region Private Methods


		[NonAction]
		private async Task ProcessAttachments(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, EmailPayloadModel emailModel, string rootUrl)
		{
			var clientId = projectDetails.Id;
			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			switch (actionItem.ActionTypeId)
			{
				case (int)ActionTypes.CostChange:
					await ProcessRevisedEstimateAttachment(actionItem, projectDetails, emailModel, rootUrl);
					break;
				case (int)ActionTypes.ScheduleChange:
					await ProcessRevisedScheduleAttachment(actionItem, projectDetails, emailModel, rootUrl);
					break;
				case (int)ActionTypes.GeneralChangeOrder:
					if(actionItem.CostChangeItemId != null)
						await ProcessRevisedEstimateAttachment(actionItem, projectDetails, emailModel, rootUrl);
					if (actionItem.ScheduleChangeItemId != null)
						await ProcessRevisedScheduleAttachment(actionItem, projectDetails, emailModel, rootUrl);
					break;

			}
			
		}

		[NonAction]
		private async Task ProcessRevisedEstimateAttachment(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, EmailPayloadModel emailModel, string rootUrl)
		{
			var clientId = projectDetails.Id;
			var revisedScheduleDocument = await this.ProcessRevisedEstimateDocument(actionItem, projectDetails, rootUrl);
			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", revisedScheduleDocument.BaseFileName, revisedScheduleDocument.Url, revisedScheduleDocument.Version, SysFolders.EstimateToActual);

			var scheduleAttachment = new AttachmentModel
			{
				FileName = revisedScheduleDocument.FileName,
				FileUrl = revisedScheduleDocument.Url,
				FileStream = revisedScheduleDocument.FileStream
			};

			emailModel.Attachments.Add(scheduleAttachment);
		}

		[NonAction]
		private async Task ProcessRevisedScheduleAttachment(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, EmailPayloadModel emailModel, string rootUrl)
		{
			var clientId = projectDetails.Id;
			var revisedScheduleDocument = await this.ProcessRevisedScheduleDocument(actionItem, projectDetails, rootUrl);
			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", revisedScheduleDocument.BaseFileName, revisedScheduleDocument.Url, revisedScheduleDocument.Version, SysFolders.Schedule);

			var scheduleAttachment = new AttachmentModel
			{
				FileName = revisedScheduleDocument.FileName,
				FileUrl = revisedScheduleDocument.Url,
				FileStream = revisedScheduleDocument.FileStream
			};

			emailModel.Attachments.Add(scheduleAttachment);
		}

		[NonAction]
		private async Task<UploadClientDocumentResult> ProcessRevisedEstimateDocument(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, string rootUrl)
		{
			var sysFolder = SysFolders.EstimateToActual;
			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			var fileContextName = "estimate-to-actual";
			var attachmentUrl = $"{rootUrl}/pdf/revised-estimate/{actionItem.ProposalId}";

			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;

			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.pdf";

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
		private async Task<UploadClientDocumentResult> ProcessRevisedScheduleDocument(ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, string rootUrl)
		{
			var sysFolder = SysFolders.ScheduleRevisions;
			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;

			var fileContextName = "schedule";
			var attachmentUrl = $"{rootUrl}/pdf/project-schedule/{clientId}";

			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;

			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.pdf";

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
		public string GetRootUrl()
		{
			var request = HttpContext.Request;
			return $"{request.Scheme}://{request.Host}";
		}

		#endregion

	}
}
