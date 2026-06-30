using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using MailKit;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Jobs
{
	public class EmailJob
	{
		private readonly IPostMarkEmailService mailService;
		private readonly IProjectsService projectsService;
		private readonly IProjectJournalService projectJournalService;
		private readonly IPDFService pDFService;
		private readonly IAzureStorageService azureStorageService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IProposalService proposalService;
		private readonly IInvoiceService invoiceService;
		private readonly ITransactionService transactionService;
		private readonly IActionItemsService actionItemsService;
		private readonly IScheduleService scheduleService;
		private readonly IRevisionsService revisionsService;
		private readonly IChangeOrderService changeOrderService;
		public readonly string tempDirectory = Path.Combine(Path.GetTempPath(), "UploadedFiles");
		private readonly IMapper mapper;
		private readonly ITenantService tenantService;
		public EmailJob(
			IPostMarkEmailService mailService, 
			IProjectsService projectsService,
			IProjectJournalService projectJournalService,
			IPDFService pDFService,
			IAzureStorageService azureStorageService,
			IClientDocumentService clientDocumentService,
			IProposalService proposalService,
			IInvoiceService invoiceService,
			ITransactionService transactionService,
			IActionItemsService actionItemsService,
			IScheduleService scheduleService,
			IRevisionsService revisionsService,
			IChangeOrderService changeOrderService,
			IMapper mapper,
			ITenantService tenantService)
		{
			this.mailService = mailService;
			this.projectsService = projectsService;
			this.projectJournalService = projectJournalService;
			this.pDFService = pDFService;
			this.azureStorageService = azureStorageService;
			this.clientDocumentService = clientDocumentService;
			this.proposalService = proposalService;
			this.invoiceService = invoiceService;
			this.transactionService = transactionService;
			this.actionItemsService = actionItemsService;
			this.scheduleService = scheduleService;
			this.revisionsService = revisionsService;
			this.changeOrderService = changeOrderService;
			this.mapper = mapper;
			this.tenantService = tenantService;
		}

		[AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
		public async Task SendAsync(EmailPayloadModel emailModel, string rootUrl, int userId, string senderName, string subDomain)
		{
			await this.SetClientDatabase(subDomain);

			this.mailService.SenderId = userId;
			this.mailService.SenderName = senderName;
			this.mailService.SubDomain = subDomain;
			Task.Run(async () =>
			{
				ProcessFileAttachments(emailModel);

				switch (emailModel.Type)
				{
					case EmailTypes.StatusReport:
						await SendStatusReport(emailModel, rootUrl);
						break;
					case EmailTypes.ProposalReport:
						await SendProposalReport(emailModel, rootUrl);
						break;
					case EmailTypes.EstimateToActualReport:
						await SendEstimateToActualReport(emailModel, rootUrl);
						break;
					case EmailTypes.ScheduleReport:
						await SendScheduleReport(emailModel, rootUrl);
						break;
					case EmailTypes.Invoice:
						await SendInvoice(emailModel, rootUrl);
						break;
					case EmailTypes.RequestDeposit:
						await SendDepositRequest(emailModel, rootUrl);
						break;
					case EmailTypes.CompleteActionItem:
						await CompleteActionItem(emailModel);
						break;
					case EmailTypes.ScheduleRevision:
						await SendScheduleRevisionReport(emailModel, rootUrl, userId);
						break;
					case EmailTypes.CostRevision:
						await SendCostRevisionReport(emailModel, rootUrl, userId);
						break;
					case EmailTypes.ChangeOrder:
						await SendChangeOrderEmail(emailModel, rootUrl, userId);
						break;
					default:
						await mailService.SendEmailAsync(emailModel);
						break;
				}
				CleanupTempDirectories();
			}).Wait();
		}

		[AutomaticRetry(Attempts = 0, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
		public async Task SendReplyAsync(EmailPayloadModel emailModel, string rootUrl, int userId, string senderName, string subDomain)
		{
			await this.SetClientDatabase(subDomain);

			this.mailService.SenderId = userId;
			this.mailService.SenderName = senderName;
			this.mailService.SubDomain = subDomain;
			Task.Run(async () =>
			{
				ProcessFileAttachments(emailModel);
				await mailService.SendReplyAsync(emailModel);
			}).Wait();
		}
		private void ProcessFileAttachments(EmailPayloadModel emailModel)
		{
			if (emailModel.AttachmentListId != null)
			{
				var attachmentListId = emailModel.AttachmentListId;
				var files = GetFilesFromTempDirectory(attachmentListId);
				if (files != null && files.Any())
				{
					emailModel.Attachments = new List<AttachmentModel>();
					foreach (var file in files)
					{
						var fileName = file.FileName;

						var attachment = new AttachmentModel
						{
							FileName = fileName,
							FileStream = file.OpenReadStream(),
							Directory = $"{DateTime.Now.ToString("dd-MM-yyyy")}/{attachmentListId}"
						};

						emailModel.Attachments.Add(attachment);
					}
				}
			}
		}
		public void CleanupTempDirectories()
		{
			if (Directory.Exists(tempDirectory))
			{
				var subDirectories = Directory.GetDirectories(tempDirectory);
				foreach (var directories in subDirectories)
				{
					try
					{
						Directory.Delete(tempDirectory, true);
					}
					catch (IOException) { }
				}
			}
		}
		private async Task<AttachmentModel> CreateInvoiceAttachment(EmailPayloadModel emailModel, Guid invoiceId, string rootUrl)
		{
			var sysFolder = SysFolders.Invoices;
			var invoiceDto = await invoiceService.GetInvoiceAsync(invoiceId);
			var clientId = invoiceDto.ClientId;
			var clientName = invoiceDto.BillTo;

			emailModel.ClientId = invoiceDto.ClientId;
			//var fileInfo = await GenerateFileInfo(clientId, clientName, "invoice");

			var attachmentUrl = $"{rootUrl}/pdf/invoice/{invoiceDto.Id}";
			var fileName = $"{clientName}-invoice-{invoiceDto.InvoiceNumber}.pdf";

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			await this.clientDocumentService.SaveClientDocument(invoiceDto.ClientId, "pdf", fileName, fileUrl, 1, sysFolder);

			return attachment;
		}
		private async Task<AttachmentModel> CreateInvoiceAttachment(Guid clientId, string clientName, InvoiceModel invoice, string rootUrl)
		{
			var sysFolder = SysFolders.Invoices;
			var invoicePayload = mapper.Map<InvoicePayload>(invoice);
			var invoiceDto = await invoiceService.CreateInvoiceAsync(invoicePayload);
			var invoiceFileInfo = await GenerateFileInfo(clientId, clientName, "invoice");
			invoiceDto.BillTo = clientName;

			var attachmentUrl = $"{rootUrl}/pdf/invoice/{invoiceDto.Id}";
			var fileName = invoiceFileInfo.FileName;

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = invoiceFileInfo.FileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", invoiceFileInfo.baseFileName, fileUrl, invoiceFileInfo.version, sysFolder);

			return attachment;
		}
		private async Task<AttachmentModel> CreateRevisedEstimateAttachment(Guid proposalId, Guid clientId, string clientName, string rootUrl)
		{
			var sysFolder = SysFolders.EstimateToActual;
			var fileContextName = "estimate-to-actual";
			var attachmentUrl = $"{rootUrl}/pdf/revised-estimate/{proposalId}";

			var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
			var fileName = fileInfo.FileName;

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, sysFolder);

			return attachment;
		}
		private async Task<AttachmentModel> CreateTransactionDetailsAttachment(Guid proposalId, Guid clientId, string clientName, string rootUrl)
		{
			//transaction details attachment
			var latestOwnerDeposit = await transactionService.GetLatestOwnerDeposit(proposalId);

			var currentDate = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc().ToString("MM/dd/yyyy");
			var startDate = latestOwnerDeposit == null ? currentDate : latestOwnerDeposit.Date.Value.ToString("MM/dd/yyyy");
			var attachmentUrl = $"{rootUrl}/pdf/transaction-details?proposalId={proposalId}&parentEstimateCategoryId=0934406a-64db-432b-a01c-5d7573a0f3ce&startDate={startDate}&endDate={currentDate}";
			var fileName = $"{clientName}-transaction-details-report-{startDate}-{currentDate}.pdf";

			var transactionDetailsPdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var transactionDetailsFileUrl = await this.azureStorageService.UploadFileFromStream(transactionDetailsPdfStream, "client-documents", fileName, $"{clientName}/transaction-detail-reports");
			var transactionDetailsAttachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = transactionDetailsFileUrl,
				FileStream = transactionDetailsPdfStream
			};

			return transactionDetailsAttachment;
		}
		private async Task<(string FileName, int version, string baseFileName)> GenerateFileInfo(Guid clientId, string clientName, string fileContextName)
		{
			var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
			var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
			var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;
			var currentDate = DateTime.UtcNow.ToString("MM-dd-yyyy");
			var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.{currentDate}.pdf";

			return (fileName, version, baseFileName);
		}
		private async Task SendStatusReport(EmailPayloadModel emailModel, string rootUrl)
		{
			if (emailModel.RefId == null) throw new Exception("Project ID is required.");

			var projectJournal = new ProjectJournalPayload
			{
				Journal = emailModel.Body
			};

			var project = await projectsService.GetProjectByIdAsync(emailModel.RefId.Value);
			if (project == null) throw new Exception("Project not found.");

			var fileContextName = "statusreport";
			var sysFolder = SysFolders.StatusReports;

			var clientId = project.Id;
			var clientName = project.Name;

			emailModel.ClientId = clientId;

			var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
			var fileName = fileInfo.FileName;

			var projectJournalResult = await this.projectJournalService.SaveAsync(emailModel.RefId.Value, projectJournal);

			var attachmentUrl = $"{rootUrl}/pdf/status-report/{projectJournalResult.Id}";

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);

			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{SysFolders.StatusReports.GetStringValue()}");

			await this.clientDocumentService.SaveClientDocument(emailModel.ClientId.Value, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, SysFolders.StatusReports);

			await mailService.SendEmailAsync(emailModel);
		}
		private async Task SendProposalReport(EmailPayloadModel emailModel, string rootUrl)
		{
			if (emailModel.RefId == null) throw new Exception("Proposal ID is required.");

			var sysFolder = SysFolders.Proposals;
			var fileContextName = "proposal";
			//var rootUrl = GetRootUrl();

			var attachmentUrl = $"{rootUrl}/pdf/proposal/{emailModel.RefId}";

			var includeZeroAmountFlag = await proposalService.CheckIfProposalIncludesZeroAmount(emailModel.RefId.Value);
			var proposal = await proposalService.GetProposalAsync(emailModel.RefId.Value, includeZeroAmountFlag);
			var project = proposal.Project;

			if (project == null) throw new Exception("Project not found.");

			var clientId = project.Id;
			var clientName = project.Name;
			emailModel.ClientId = clientId;

			var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
			var fileName = fileInfo.FileName;

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.mailService.SendEmailAsync(emailModel);
			//var emailSent = await this.postMarkEmailService.SendEmailAsync(emailModel);

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, sysFolder);
		}
		private async Task SendEstimateToActualReport(EmailPayloadModel emailModel, string rootUrl)
		{
			if (emailModel.RefId == null) throw new Exception("Proposal ID is required.");

			var sysFolder = SysFolders.EstimateToActual;
			var fileContextName = "estimate-to-actual";
			var attachmentUrl = $"{rootUrl}/pdf/revised-estimate/{emailModel.RefId}";

			var proposal = await proposalService.GetProposalAsync(emailModel.RefId.Value);
			var project = proposal.Project;

			if (project == null) throw new Exception("Project not found.");

			var clientId = project.Id;
			var clientName = project.Name;

			emailModel.ClientId = clientId;

			var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
			var fileName = fileInfo.FileName;

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.mailService.SendEmailAsync(emailModel);

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, sysFolder);
		}
		private async Task SendScheduleReport(EmailPayloadModel emailModel, string rootUrl)
		{
			if (emailModel.RefId == null) throw new Exception("Project ID is required.");

			var project = await projectsService.GetProjectByIdAsync(emailModel.RefId.Value);
			if (project == null) throw new Exception("Project not found.");

			var sysFolder = SysFolders.Schedule;
			var fileContextName = "schedule";
			var attachmentUrl = $"{rootUrl}/pdf/project-schedule/{emailModel.RefId}";

			var clientId = project.Id;
			var clientName = project.Name;

			emailModel.ClientId = clientId;

			var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
			var fileName = fileInfo.FileName;

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.mailService.SendEmailAsync(emailModel);

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, sysFolder);
		}
		private async Task SendInvoice(EmailPayloadModel emailModel, string rootUrl)
		{
			if (emailModel.RefId == null) throw new Exception("Invoice ID is required.");

			var attachment = await CreateInvoiceAttachment(emailModel, emailModel.RefId.Value, rootUrl);

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			emailModel.Attachments.Add(attachment);

			var emailSent = await this.mailService.SendEmailAsync(emailModel);
		}
		private async Task SendDepositRequest(EmailPayloadModel emailModel, string rootUrl)
		{
			if (emailModel.RefId == null) throw new Exception("Proposal Id is required");

			var proposalId = emailModel.RefId.Value;
			var proposal = await proposalService.GetProposalAsync(proposalId);
			var project = proposal.Project;

			if (project == null) throw new Exception("Project not found.");

			var clientId = project.Id;
			var clientName = project.Name;

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			var revisedEstimateAttachment = await CreateRevisedEstimateAttachment(proposalId, clientId, clientName, rootUrl);
			emailModel.Attachments.Add(revisedEstimateAttachment);

			var invoiceAttachment = await CreateInvoiceAttachment(clientId, clientName, emailModel.Invoice, rootUrl);
			emailModel.Attachments.Add(invoiceAttachment);

			var transactionDetailsAttachment = await CreateTransactionDetailsAttachment(proposalId, clientId, clientName, rootUrl);
			emailModel.Attachments.Add(transactionDetailsAttachment);

			var emailSent = await this.mailService.SendEmailAsync(emailModel);
		}
		private async Task CompleteActionItem(EmailPayloadModel emailModel)
		{
			if (emailModel.Ref2Id == null) throw new Exception("Action Item ID is required.");
			await this.actionItemsService.CompleteActionItemAsync(emailModel.Ref2Id.Value);
			await this.mailService.SendEmailAsync(emailModel);

		}
		private async Task SendScheduleRevisionReport(EmailPayloadModel emailModel, string rootUrl, int userId)
		{
			if (emailModel.RefId == null) throw new Exception("Project ID is required.");
			if (emailModel.AdditionalRefId == null) throw new Exception("Schedule Revision ID is required.");

			var project = await projectsService.GetProjectByIdAsync(emailModel.RefId.Value);
			if (project == null) throw new Exception("Project not found.");

			var clientId = project.Id;
			var clientName = project.Name;
			var scheduleRevisionId = emailModel.AdditionalRefId.Value;

			emailModel.ClientId = clientId;

			if (emailModel.Attachments == null) emailModel.Attachments = new List<AttachmentModel>();

			var scheduleAttachment = await CreateScheduleAttachment(clientId, clientName, rootUrl);
			emailModel.Attachments.Add(scheduleAttachment);

			var scheduleRevision = await scheduleService.GetScheduleRevision(scheduleRevisionId);
			var scheduleRevisionAttachment = await CreateScheduleRevisionAttachment(scheduleRevision, clientId, clientName, rootUrl);
			emailModel.Attachments.Add(scheduleRevisionAttachment);

			//create action item note
			await CreateActionItemForScheduleRevision(scheduleRevision, clientId, clientName, userId);

			// Define the marker for where you want to insert the button
			string marker = "Best regards,";

			// Define your button HTML
			var linkUrl = $"{rootUrl}/approvals/schedule-revisions/{scheduleRevisionId}";
			string buttonHtml = @$"<p><a href=""{linkUrl}"" rel=""noopener"" style=""width:100px;text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">Acknowledge</a></p>";

			// Find the index of the marker
			int insertIndex = emailModel.Body.IndexOf(marker);

			// Insert the button before "Best regards,"
			string updatedBody = emailModel.Body.Insert(insertIndex, buttonHtml + "\n\n");
			emailModel.Body = updatedBody;

			var emailSent = await this.mailService.SendEmailAsync(emailModel);

			if (emailSent)
				await revisionsService.MarkScheduleRevisionAsSent(scheduleRevision.Id);
		}
		private async Task<AttachmentModel> CreateScheduleAttachment(Guid clientId, string clientName, string rootUrl)
		{
			var sysFolder = SysFolders.Schedule;
			var fileContextName = "schedule";
			var attachmentUrl = $"{rootUrl}/pdf/project-schedule/{clientId}";

			var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
			var fileName = fileInfo.FileName;

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, sysFolder);

			return attachment;
		}
		private async Task<AttachmentModel> CreateScheduleRevisionAttachment(ScheduleRevisionDto scheduleRevision, Guid clientId, string clientName, string rootUrl)
		{
			var revisionId = scheduleRevision.Id;
			var attachmentUrl = $"{rootUrl}/pdf/schedule-revision/{revisionId}";
			var fileName = $"{clientName}-schedule-revision-{scheduleRevision.RevisionNumber.ToString().PadLeft(5, '0')}.pdf";

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{SysFolders.ScheduleRevisions.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", "schedule-revision", fileUrl, 1, SysFolders.ScheduleRevisions);

			return attachment;
		}
		private async Task CreateActionItemForScheduleRevision(ScheduleRevisionDto scheduleRevision, Guid clientId, string clientName, int userId)
		{
			//create an action item note here
			var actionItemDescription = "<p>FOR CLIENT ACKNOWLEDGEMENT</p>";
			foreach (var revisionItem in scheduleRevision.ScheduleRevisionItems)
			{
				actionItemDescription += $"<p>";
				actionItemDescription += $"Task: {revisionItem.ConstructionTask}<br/>";
				actionItemDescription += $"Reason: {revisionItem.Reason}<br/>";
				actionItemDescription += $"Old Duration: {revisionItem.OldDuration}<br/>";
				actionItemDescription += $"New Duration: {revisionItem.NewDuration}<br/>";
				actionItemDescription += $"</p>";
			}

			var actionItemPayload = new List<ActionItemPayload> {
				new ActionItemPayload{
					ActionTypeId = (int)ActionTypes.Note,
					Description = actionItemDescription,
					DueDate = DateOnly.FromDateTime(TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()),
						ProjectId = clientId,
					Supervisors = new List<int>() { userId },
					Title = $"Schedule Revision - {clientName} - {scheduleRevision.RevisionNumber.ToString().PadLeft(5,'0')}",
					Status = (int)ActionItemStatus.PendingClientAcknowledgement,

				}
			};

			actionItemsService.UserId = userId;
			var createdActionItems = await actionItemsService.CreateActionItems(actionItemPayload);
			var createdActionItemId = createdActionItems.FirstOrDefault().Id;
			await revisionsService.AssignActionItemToScheduleRevision(scheduleRevision.Id, createdActionItemId);
		}
		private async Task SendCostRevisionReport(EmailPayloadModel emailModel, string rootUrl, int userId)
		{
			if (emailModel.RefId == null) throw new Exception("Proposal ID is required.");
			if (emailModel.AdditionalRefId == null) throw new Exception("Revision ID is required.");

			var proposal = await proposalService.GetProposalAsync(emailModel.RefId.Value);
			var project = proposal.Project;

			if (project == null) throw new Exception("Project not found.");

			var proposalId = emailModel.RefId.Value;
			var revisionId = emailModel.AdditionalRefId.Value;
			var clientId = project.Id;
			var clientName = project.Name;

			emailModel.ClientId = clientId;

			if (emailModel.Attachments == null)
				emailModel.Attachments = new List<AttachmentModel>();

			var revisedEstimateAttachment = await CreateRevisedEstimateAttachment(proposalId, clientId, clientName, rootUrl);
			emailModel.Attachments.Add(revisedEstimateAttachment);

			var costRevisionDto = await revisionsService.GetCostRevisionAsync(revisionId);
			var costRevisionAttachment = await CreateCostRevisionAttachment(costRevisionDto, clientId, clientName, rootUrl);
			emailModel.Attachments.Add(costRevisionAttachment);

			//create action item note
			await CreateActionItemForCostRevision(costRevisionDto, clientId, clientName, userId);

			// Define the marker for where you want to insert the button
			string marker = "Best regards,";

			// Define your button HTML
			var linkUrl = $"{rootUrl}/approvals/cost-revisions/{revisionId}";
			string buttonHtml = @$"<p><a href=""{linkUrl}"" rel=""noopener"" style=""width:100px;text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">Acknowledge</a></p>";

			// Find the index of the marker
			int insertIndex = emailModel.Body.IndexOf(marker);

			// Insert the button before "Best regards,"
			string updatedBody = emailModel.Body.Insert(insertIndex, buttonHtml + "\n\n");
			emailModel.Body = updatedBody;

			var emailSent = await this.mailService.SendEmailAsync(emailModel);
		}
		private async Task<AttachmentModel> CreateCostRevisionAttachment(CostRevisionDto costRevisionDto, Guid clientId, string clientName, string rootUrl)
		{
			var revisionId = costRevisionDto.Id;
			var sysFolder = SysFolders.CostRevisions;
			var attachmentUrl = $"{rootUrl}/pdf/cost-revision/{revisionId}";

			var fileName = $"{clientName}-cost-revision-{costRevisionDto.RevisionNumber.ToString().PadLeft(5, '0')}.pdf";

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			var attachment = new AttachmentModel
			{
				FileName = fileName,
				FileUrl = fileUrl,
				FileStream = pdfStream
			};

			await this.clientDocumentService.SaveClientDocument(clientId, "pdf", "cost-revision", fileUrl, 1, sysFolder);
			return attachment;
		}
		private async Task CreateActionItemForCostRevision(CostRevisionDto costRevisionDto, Guid clientId, string clientName, int userId)
		{
			//create an action item note here
			var actionItemDescription = "<p>FOR CLIENT ACKNOWLEDGEMENT</p>";
			foreach (var revisionItem in costRevisionDto.CostRevisionItems)
			{
				actionItemDescription += $"<p>";
				actionItemDescription += $"Estimate Category: {revisionItem.EstimateCategory}<br/>";
				actionItemDescription += $"Amount: {revisionItem.Amount}<br/>";
				actionItemDescription += $"Current Amount: {revisionItem.CurrentAmount}<br/>";
				actionItemDescription += $"New Amount: {revisionItem.NewAmount}<br/>";
				actionItemDescription += $"</p>";
			}

			var actionItemPayload = new List<ActionItemPayload> {
				new ActionItemPayload{
					ActionTypeId = (int)ActionTypes.Note,
					Description = actionItemDescription,
					DueDate = DateOnly.FromDateTime(TimezoneUtils.GetDefaultCaliforniaTimezoneUtc()),
						ProjectId = clientId,
					Supervisors = new List<int>() { userId },
					Title = $"Cost Revision - {clientName} - {costRevisionDto.RevisionNumber.ToString().PadLeft(5,'0')}",
					Status = (int)ActionItemStatus.PendingClientAcknowledgement,

				}
			};

			actionItemsService.UserId = userId;
			var createdActionItems = await actionItemsService.CreateActionItems(actionItemPayload);
			var createdActionItemId = createdActionItems.FirstOrDefault().Id;
			await revisionsService.AssignActionItemToCostRevision(costRevisionDto.Id, createdActionItemId);
		}
		private async Task SendChangeOrderEmail(EmailPayloadModel emailModel, string rootUrl, int userId)
		{
			if (emailModel.Ref2Id == null) throw new Exception("Action Item ID is required.");

			var actionItem = await actionItemsService.GetActionItemSummaryViewByIdAsync(emailModel.Ref2Id.Value);
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

			var clientDocument = await UploadChangeOrderDocument(changeOrder, actionItem, projectDetails, sysFolder, rootUrl);

			// Define the marker for where you want to insert the button
			string marker = "Best regards,";

			// Define your button HTML
			var linkUrl = $"{rootUrl}/approvals/cost-change/{emailModel.Ref2Id}";
			string buttonHtml = @$"<p><a href=""{linkUrl}"" rel=""noopener"" style=""width:100px;text-decoration:none;display:inline-block;text-align:center;padding:0.75575rem 1.3rem;font-size:0.925rem;line-height:1.5;border-radius:0.35rem;color:#ffffff;background-color:#009ef7;border:0px;margin-right:0.75rem!important;font-weight:600!important;outline:none!important;vertical-align:middle"" target=""_blank"">Approve</a></p>";

			// Find the index of the marker
			int insertIndex = emailModel.Body.IndexOf(marker);

			// Insert the button before "Best regards,"
			string updatedBody = emailModel.Body.Insert(insertIndex, buttonHtml + "\n\n");
			emailModel.Body = updatedBody;

			await SendChangeOrderApprovalEmail(emailModel, actionItem, projectDetails, clientDocument);
			await this.clientDocumentService.SaveClientDocument(projectDetails.Id, "pdf", clientDocument.BaseFileName, clientDocument.Url, clientDocument.Version, sysFolder);

		}
		private async Task<UploadClientDocumentResult> UploadChangeOrderDocument(ChangeOrderDto changeOrder, ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, SysFolders sysFolder, string rootUrl)
		{
			var clientId = projectDetails.Id;
			var clientName = projectDetails.Name;
			var fileName = $"{clientName}.change-order-{changeOrder.ChangeOrderNumber.ToString().PadLeft(5, '0')}.pdf";

			var attachmentUrl = $"{rootUrl}/pdf/change-order/{actionItem.Id}";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
			var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

			return new UploadClientDocumentResult
			{
				FileName = fileName,
				Url = fileUrl,
				FileStream = pdfStream
			};
		}
		private async Task SendChangeOrderApprovalEmail(EmailPayloadModel emailModel, ActionItemSummaryViewDto actionItem, ProjectDetailsDto projectDetails, UploadClientDocumentResult clientDocument)
		{
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

			var emailSent = await this.mailService.SendEmailAsync(emailModel);
		}
		public List<IFormFile> GetFilesFromTempDirectory(string id)
		{
			var directoryPath = Path.Combine(tempDirectory, id);
			var files = new List<IFormFile>();

			if (Directory.Exists(directoryPath))
			{
				var filePaths = Directory.GetFiles(directoryPath);
				foreach (var filePath in filePaths)
				{
					var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
					var formFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(filePath))
					{
						Headers = new HeaderDictionary(),
						ContentType = "application/octet-stream"
					};

					files.Add(formFile);
				}
			}

			return files;
		}

		private async Task SetClientDatabase(string subDomain)
		{
			var clientDatabase = await tenantService.GetClientDatabase(subDomain);

			this.mailService.ClientDbContext = clientDatabase;
			this.azureStorageService.ClientDbContext = clientDatabase;
			this.clientDocumentService.ClientDbContext = clientDatabase;
			this.actionItemsService.ClientDbContext = clientDatabase;
			this.invoiceService.ClientDbContext = clientDatabase;
			this.revisionsService.ClientDbContext = clientDatabase;
			this.scheduleService.ClientDbContext = clientDatabase;
			this.projectsService.ClientDbContext = clientDatabase;
			this.proposalService.ClientDbContext = clientDatabase;
			this.projectJournalService.ClientDbContext = clientDatabase;
			this.transactionService.ClientDbContext = clientDatabase;
		}
	}
}
