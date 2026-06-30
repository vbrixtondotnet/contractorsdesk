using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Helpers;
using ContractorsDesk.WebPortal.Jobs;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostmarkDotNet.Model;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/email")]
	[ApiController]
	[RequestSizeLimit(52428800)]
	public class EmailController : BaseApiController
	{
		private readonly IProposalService proposalService;
		private readonly IProjectsService projectsService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IProjectJournalService projectJournalService;
		private readonly IInvoiceService invoiceService;
		private readonly IPDFService pDFService;
		private readonly IActionItemsService actionItemsService;
		private readonly IRevisionsService revisionsService;
		private readonly ITransactionService transactionService;
		private readonly IScheduleService scheduleService;
		private readonly IMapper mapper;
		private readonly IChangeOrderService changeOrderService;
		private readonly IPostMarkEmailService mailService;
		private readonly IEmailsService emailsService;
		private readonly TenantResolver tenantResolver;
		public EmailController(
			IPostMarkEmailService mailService,
			IEmailsService emailService,
			IProposalService proposalService,
			IProjectsService projectsService,
			IAzureStorageService azureStorageService,
			IClientDocumentService clientDocumentService,
			IProjectJournalService projectJournalService,
			IActionItemsService actionItemsService,
			ITransactionService transactionService,
			IRevisionsService revisionsService,
			IPDFService pDFService,
			IScheduleService scheduleService,
			IInvoiceService invoiceService,
            IChangeOrderService changeOrderService,
            IMapper mapper,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor,
			TenantResolver tenantResolver
			) : base(httpContextAccessor,permissionsService,applicationUserService, azureStorageService)
		{
			this.mailService = mailService;
			this.emailsService = emailService;
			this.projectsService = projectsService;
			this.azureStorageService = azureStorageService;
			this.clientDocumentService = clientDocumentService;
			this.projectJournalService = projectJournalService;
			this.pDFService = pDFService;
			this.proposalService = proposalService;
			this.invoiceService = invoiceService;
			this.actionItemsService = actionItemsService;
			this.revisionsService = revisionsService;
			this.transactionService = transactionService;
			this.scheduleService = scheduleService;
			this.changeOrderService = changeOrderService;

			this.actionItemsService.UserId = this.UserId;
			this.mailService.SenderId = this.UserId;
			this.mapper = mapper;
			this.tenantResolver = tenantResolver;
			this.mailService.SubDomain = this.tenantResolver.GetSubDomain();
		}

		#region GET
		[HttpGet("sent-items")]
		public async Task<IActionResult> GetSentItems()
		{
			var response = await emailsService.GetUserSentItemsAsync(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs);
			return Ok(ApiResponse<UserInboxDto>.SuccessResponse(response));
		}

		[HttpGet("sent-email/{id}")]
		public async Task<IActionResult> GetSentEmailById(Guid id)
		{
			var response = await mailService.GetSentEmailByIdAsync(id);
			return Ok(ApiResponse<SentEmailDto>.SuccessResponse(response));
		}

        [HttpGet("to-email-addresses")]
        public async Task<IActionResult> GetToEmailAddresses()
        {
            var response = await emailsService.GetToEmailAddresses();
            return Ok(ApiResponse<List<string>>.SuccessResponse(response));
		}

		[HttpGet("inbox")]
		public async Task<IActionResult> GetUserInbox()
		{
			var response = await emailsService.GetUserInboxAsync(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs);
			return Ok(ApiResponse<UserInboxDto>.SuccessResponse(response));
		}
		#endregion

		#region POST
		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPost("attachment")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string emailId)
		{
			if (file == null || file.Length == 0) return BadRequest("No file uploaded.");
			if (string.IsNullOrEmpty(emailId)) return BadRequest("Email ID is required.");

			await SaveFileToTempDirectory(file, emailId);
			return Ok("File uploaded successfully.");
		}

		[HttpPost("send")]
		public IActionResult SendEmail([FromBody] EmailPayloadModel emailModel)
		{
			var rootUrl = GetRootUrl();
			var userId = this.UserId;
			var senderName = $"{this.CurrentUser.FirstName} {this.CurrentUser.LastName}";
			var subDomain = tenantResolver.GetSubDomain();
			BackgroundJob.Enqueue<EmailJob>(job => job.SendAsync(emailModel, rootUrl, userId, senderName, subDomain));
			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		[HttpPost("reply")]
		public IActionResult SendReply([FromBody] EmailPayloadModel emailModel)
		{
			var rootUrl = GetRootUrl();
			var userId = this.UserId;
			var senderName = $"{this.CurrentUser.FirstName} {this.CurrentUser.LastName}";
			var subDomain = tenantResolver.GetSubDomain();
			BackgroundJob.Enqueue<EmailJob>(job => job.SendReplyAsync(emailModel, rootUrl, userId, senderName, subDomain));
			return Ok(ApiResponse<bool>.SuccessResponse(true));
		}

		#endregion

		#region PUT
		[HttpPut("sent-email-read/{id}")]
		public async Task<IActionResult> MarkSentEmailAsRead(Guid id)
		{
			await mailService.MarkSentEmailAsRead(id);

			return Ok(ApiResponse<Guid>.SuccessResponse(id));
		}

		#endregion

		#region PATCH
		[HttpPatch("inbox/mark-read/{messageId}")]
		public async Task<IActionResult> MarkInboxEmailAsRead(string messageId)
		{
			await emailsService.MarkAsReadAsync(messageId);

			return Ok(ApiResponse<string>.SuccessResponse(messageId));
		}

		[HttpPatch("inbox/bulk-mark-read")]
		public async Task<IActionResult> BulkMarkInboxEmailAsRead([FromBody] List<string> messageIds)
		{
			foreach (var id in messageIds)
			{
				await emailsService.MarkAsReadAsync(id);
			}

			return Ok(ApiResponse<List<string>>.SuccessResponse(messageIds));
		}

		[HttpPatch("inbox/{messageId}/archive")]
		public async Task<IActionResult> Archive(string messageId)
		{
			await emailsService.ArchiveAsync(messageId);

			return Ok(ApiResponse<string>.SuccessResponse(messageId));
		}

		[HttpPatch("inbox/bulk-archive")]
		public async Task<IActionResult> BulkArchive([FromBody] List<string> messageIds)
		{
			foreach (var id in messageIds)
			{
				await emailsService.ArchiveAsync(id);
			}

			return Ok(ApiResponse<List<string>>.SuccessResponse(messageIds));
		}
		#endregion

		#region DELETE
		[HttpDelete("attachment")]
		public IActionResult DeleteFile(string fileName, string attachmentListId)
		{
			var filePath = Path.Combine(tempDirectory, attachmentListId, fileName);
			if (System.IO.File.Exists(filePath))
			{
				System.IO.File.Delete(filePath);
				return Ok("File deleted successfully.");
			}

			return NotFound("File not found.");
		}

		[HttpDelete("inbox/{messageId}")]
		public async Task<IActionResult> Delete(string messageId)
		{
			await emailsService.DeleteAsync(messageId);

			return Ok(new { Id = messageId.ToString() });
		}

		[HttpDelete("inbox")]
		public async Task<IActionResult> BulkDelete([FromBody] List<string> messageIds)
		{
			foreach (var id in messageIds)
			{
				await emailsService.DeleteAsync(id);
			}

			return Ok(new { Id = messageIds.ToString() });
		}
		#endregion
	}
}
