using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class ScheduleController : BaseApiController
	{
		private readonly IScheduleService scheduleService;
		private readonly IConstructionTasksService constructionTasksService;
		private readonly IMapper mapper;
		private readonly IProjectsService projectService;
		private readonly IClientDocumentService clientDocumentService;
		private readonly IPDFService pDFService;

        public ScheduleController(
			IScheduleService scheduleService,
			IProjectsService projectService,
			IClientDocumentService clientDocumentService,
			IConstructionTasksService constructionTasksService,
			IPermissionsService permissionsService,
			IAzureStorageService azureStorageService,
			IApplicationUserService applicationUserService,
			IPDFService pDFService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService) 
		{
			this.mapper = mapper;
			this.projectService = projectService;
			this.scheduleService = scheduleService;
			this.constructionTasksService = constructionTasksService;
			this.scheduleService.UserId = this.UserId;
			this.clientDocumentService = clientDocumentService;
			this.pDFService = pDFService;
			this.azureStorageService = azureStorageService;
		}

		#region GET

		[AllowAnonymous]
		[HttpGet("schedule")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetConstructionTasks()
		{
			var constructionTasks = await this.constructionTasksService.GetAllConstructionTaskAsync();

			return Ok(ApiResponse<List<ConstructionTaskDto>>.SuccessResponse(constructionTasks));

		}

		[HttpGet("schedule/{projectId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectSchedule(Guid projectId)
		{
			var projectSchedule = await this.scheduleService.GetProjectScheduleAsync(projectId);

			return Ok(ApiResponse<ProjectScheduleViewDto>.SuccessResponse(projectSchedule));

		}

		[HttpGet("unschedule/{projectId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetUnscheduledProposalLines(Guid projectId)
		{
			var unmappedProposalLines = await this.scheduleService.GetUnmappedProposalLines(projectId);
			return Ok(ApiResponse<List<ProposalLineItemDto>>.SuccessResponse(unmappedProposalLines));

		}

		[HttpGet("schedule/{projectId}/construction-tasks")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectScheduleAsync(Guid projectId)
		{
			var projectSchedule = await this.scheduleService.GetProjectScheduleConstructionTasks(projectId);

			return Ok(ApiResponse<ProjectScheduleDto>.SuccessResponse(projectSchedule));
		}
	
		#endregion

		#region PUT
		[HttpPut("schedule/{projectId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveProjectSchedule(Guid projectId, ProjectSchedulePayload payload)
		{
			var projectSchedule = await this.scheduleService.SaveProjectScheduleAsync(projectId, payload);
			return Ok(ApiResponse<ProjectScheduleViewDto>.SuccessResponse(projectSchedule));
		}


		[HttpPut("schedule/{projectId}/delay")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveProjectSchedule(Guid projectId, ProjectScheduleDelayDto projectScheduleDelayDto)
		{
			var response = await this.scheduleService.AddProjectDelay(projectId, projectScheduleDelayDto);
			return Ok(ApiResponse<ProjectScheduleDelayDto>.SuccessResponse(response));

		}
        #endregion

        [NonAction]
		[AutomaticRetry(Attempts = 0)]
		public async Task GenerateScheduleReport(string rootUrl, Guid projectId)
        {
            var project = await this.projectService.GetProjectByIdAsync(projectId);
            if (project == null) throw new Exception("Project not found.");

            var sysFolder = SysFolders.Schedule;
            var fileContextName = "schedule";
            var attachmentUrl = $"{rootUrl}/pdf/project-schedule/{projectId}";

            var clientId = project.Id;
            var clientName = project.Name;

            var fileInfo = await GenerateFileInfo(clientId, clientName, fileContextName);
            var fileName = fileInfo.FileName;

            var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(attachmentUrl);
            var fileUrl = await this.azureStorageService.UploadFileFromStream(pdfStream, "client-documents", fileName, $"{clientName}/{sysFolder.GetStringValue()}");

            await this.clientDocumentService.SaveClientDocument(clientId, "pdf", fileInfo.baseFileName, fileUrl, fileInfo.version, sysFolder);
        }

        [NonAction]
        private async Task<(string FileName, int version, string baseFileName)> GenerateFileInfo(Guid clientId, string clientName, string fileContextName)
        {
            var baseFileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.pdf";
            var latestDocumentVersion = await this.clientDocumentService.GetDocumentLatestVersionByFileNameAsync(clientId, baseFileName);
            var version = latestDocumentVersion == null ? 1 : latestDocumentVersion.Value + 1;
            var currentDate = DateTime.UtcNow.ToString("MM-dd-yyyy");
            var fileName = $"{clientName.Replace(" ", "-")}.{fileContextName}.v{version}.{currentDate}.pdf";

            return (fileName, version, baseFileName);
        }

        [NonAction]
        private string GetRootUrl()
        {
            var request = HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }

    }
}
