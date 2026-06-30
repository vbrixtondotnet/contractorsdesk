using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using DocuSign.eSign.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SignNow.Net.Model.Requests.GetFolderQuery;
using System;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/upload")]
	[ApiController]
	[RequestSizeLimit(209715200)]
	public class FileUploadController : BaseApiController
	{
		private readonly IAudioUploadsService userUploadsService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IProjectsService projectsService;
		private readonly INotificationService notificationService;
		private readonly ICompanySettingService companySettingService;
		private readonly ISubcontractorsService subcontractorsService;
		public FileUploadController(
			IAzureStorageService azureStorageService,
			IAudioUploadsService userUploadsService,
			IClientDocumentService clientDocumentService,
			IProjectsService projectsService,
			INotificationService notificationService,
			ICompanySettingService companySettingService,
			ISubcontractorsService subcontractorsService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService)
		{
			this.azureStorageService = azureStorageService;
			this.userUploadsService = userUploadsService;
			this.clientDocumentService = clientDocumentService;
			this.projectsService = projectsService;
			this.notificationService = notificationService;
			this.companySettingService = companySettingService;
			this.subcontractorsService = subcontractorsService;
		}

		[HttpPost("audio")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> UploadAudioFile([FromForm] IFormFile audioFile)
		{
			var fileUrl = await azureStorageService.UploadFile(audioFile, "action items", audioFile.FileName, "audio files");

			var userUploadDto = new UserUploadDto
			{
				Id = Guid.NewGuid(),
				Aistatus = "Pending",
				DateCreated = DateTime.UtcNow,
				StorageStatus = "Success",
				FileUrl = fileUrl,
				UserId = this.UserId
			};
			await userUploadsService.CreateAsync<UserUploadDto>(userUploadDto);

			var userIds = new List<int> { this.UserId };
			var notificationCategory = NotificationCategory.AI.GetStringValue();
			var message = NotificationMessage.ProcessedByAI.GetStringValue();
			string notificationMessage = "AI is currently processing your audio. Please wait for more updates.";

			var notificationRequest = await notificationService.GenerateUserNotificationPayloads(notificationCategory, notificationMessage, "", 1, userIds: userIds);

			await notificationService.AddNewNotification(notificationRequest);

			return Ok(ApiResponse<UserUploadDto>.SuccessResponse(userUploadDto));
		}

		[HttpPost("file")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string storageName, [FromForm] string directory, [FromForm] Guid folderId, [FromForm] Guid clientId, [FromForm] Guid? subcontractorId = null)
		{
			var project = await this.projectsService.GetProjectShortDetailsAsync(clientId);
			var clientName = project.Name;
			var directoryName = $"{clientName}/{directory}";
			

			var fileUrl = await azureStorageService.UploadFile(file, storageName, file.FileName, directoryName);

			var fileExtension = Path.GetExtension(file.FileName).Replace(".", "");
			var clientDocument = new ClientDocumentDto
			{
				ClientId = clientId,
				FileExtension = fileExtension,
				FileName = file.FileName,
				Url = fileUrl,
				Version = 1,
				DateCreated = TimezoneUtils.GetDefaultCaliforniaTimezoneUtc(),
			};

			clientDocument.SubcontractorId = subcontractorId;
			var result = await this.clientDocumentService.SaveClientDocument(clientDocument, folderId);

			return Ok(ApiResponse<ClientDocumentDto>.SuccessResponse(result));
		}

		[HttpPost("company-logo")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> UploadCompanyLogo([FromForm] IFormFile file)
		{
			var companySettings = await this.companySettingService.GetCompanySettingAsync();
			var directoryName = $"{companySettings.CompanyName}/company-logo";
			var fileUrl = await azureStorageService.UploadFile(file, "companyfiles", file.FileName, directoryName);
			companySettings.CompanyLogoUrl = fileUrl;
			await this.companySettingService.SaveCompanySettingAsync(companySettings);
			
			return Ok(ApiResponse<CompanySettingDto>.SuccessResponse(companySettings));
		}
	}
}
