using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/projects")]
	[ApiController]
	public class ProjectsController : BaseApiController
	{
		private readonly IProjectsService projectsService;
		private readonly IScheduleService scheduleService;
		private readonly IProjectJournalService projectJournalService;
		private readonly ISubcontractorsService subcontractorsService;
		private readonly IMapper mapper;
		public ProjectsController(
			IProjectsService projectsService,
			IProposalService proposalService,
			IScheduleService scheduleService,
			IProjectJournalService projectJournalService,
			ISubcontractorsService subcontractorsService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IAzureStorageService azureStorageService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService: azureStorageService) 
		{
			this.mapper = mapper;	
			this.projectsService = projectsService;
			this.scheduleService = scheduleService;
			this.projectJournalService = projectJournalService;
			this.subcontractorsService = subcontractorsService;
			this.projectsService.UserId = this.UserId;
		}

		#region GET
		[HttpGet]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjects()
		{
			var response = await projectsService.GetActiveJobsByUserAsync(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs);
			return Ok(ApiResponse<List<ProjectDetailsDto>>.SuccessResponse(response));
		}

		[HttpGet("archived")]
		public async Task<IActionResult> GetArchivedProjects()
		{
			var response = await projectsService.GetArchivedJobsByUserAsync(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs);
			return Ok(ApiResponse<List<ProjectDetailsDto>>.SuccessResponse(response));
		}

		[HttpGet("search")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SearchProject(string search)
		{
			var response = await projectsService.GetActiveJobsByUserAsync(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs, search);
			return Ok(ApiResponse<List<ProjectDetailsDto>>.SuccessResponse(response));
		}

		[HttpGet("active-jobs")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActiveJobs()
		{
			var response = await projectsService.GetActiveProjectsAsync();
			return Ok(ApiResponse<List<ProjectDto>>.SuccessResponse(response));
		}

		[HttpGet("supervisor-active-jobs")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetSupervisorActiveJobs()
		{
			var response = await projectsService.GetSupervisorActiveJobs(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs);
			return Ok(ApiResponse<List<ActiveProjectDto>>.SuccessResponse(response));
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProject(Guid id)
		{
			var response = await projectsService.GetProjectDetailsByIdAsync(id);
			return Ok(ApiResponse<ProjectDetailsViewDto>.SuccessResponse(response));
		}

		[HttpGet("{id}/short-details")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectShortDetails(Guid id)
		{
			var response = await projectsService.GetProjectShortDetailsAsync(id);
			return Ok(ApiResponse<ProjectDetailsDto>.SuccessResponse(response));
		}

		[HttpGet("{id}/sub-contractors")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectSubContractors(Guid id)
		{
			var response = await subcontractorsService.GetSubContractorsByProjectIdAsync(id);
			return Ok(ApiResponse<List<SubContractorDto>>.SuccessResponse(response));
		}

		[HttpGet("{id}/proposal")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectProposal(Guid id)
		{
			var response = await projectsService.GetProjectProposal(id);
			return Ok(ApiResponse<ProposalDto>.SuccessResponse(response));
		}

		[HttpGet("{id}/supervisors")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectSupervisors(Guid id)
		{
			var projectSupervisors = await projectsService.GetProjectSupervisorsAsync(id);
			return Ok(ApiResponse<List<ProjectSupervisorDto>>.SuccessResponse(projectSupervisors));
		}

		[HttpGet("{id}/project-journal")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectJournal(Guid id)
		{
			var projectJournal = await projectJournalService.GetProjectJournalByProjectIdAsync(id);
			return Ok(ApiResponse<ProjectJournalDto>.SuccessResponse(projectJournal));
		}

		[AllowAnonymous]
		[HttpGet("{id}/schedule")]
		[ProducesResponseType(typeof(ApiResponse<ProjectScheduleDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectSchedule(Guid id)
		{
			var response = await scheduleService.GetProjectScheduleAsync(id);
			return Ok(ApiResponse<ProjectScheduleViewDto>.SuccessResponse(response));
		}

		[HttpGet("{id}/estimate-categories-and-schedules")]
		[ProducesResponseType(typeof(ApiResponse<ProjectEstimateCategoriesAndScheduleItemsDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjectEstimateCategoriesAndScheduleItemsAsync(Guid id)
		{
			var response = await projectsService.GetProjectEstimateCategoriesAndScheduleItemsAsync(id);
			return Ok(ApiResponse<ProjectEstimateCategoriesAndScheduleItemsDto>.SuccessResponse(response));
		}

		#endregion

		#region POST

		[HttpPost("{id}/sub-contractor")]
		[ProducesResponseType(typeof(ApiResponse<SubContractorDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> AddSubcontractor([FromBody] SubContractorPayloadModel model)
		{			
			var result = await projectsService.AddSubcontractorAsync(model);
			return Ok(ApiResponse<SubContractorDto>.SuccessResponse(result));
		}

        #endregion

        #region PUT
        [HttpPut("{id}")]
		public async Task<IActionResult> UpdateProject(Guid id, [FromBody] ProjectPayload project)
		{
			project.Id = id;
			await projectsService.UpdateAsync<ProjectDto>(project);
			var response = await projectsService.GetProjectDetailsByIdAsync(id);
			return Ok(ApiResponse<ProjectDetailsViewDto>.SuccessResponse(response));
		}

		[HttpPut("{id}/client")]
		[ProducesResponseType(typeof(ApiResponse<ProjectClientDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateProjectClient(Guid id, [FromBody] ClientModel clientModel)
		{
			var response = await projectsService.UpdateClientDetailsAsync(id, clientModel);
			return Ok(ApiResponse<ClientDto>.SuccessResponse(response));
		}

		[HttpPut("{id}/project-journal")]
		public async Task<IActionResult> SaveProjectJournal(Guid id, ProjectJournalPayload payload)
		{
			await projectJournalService.SaveAsync(id, payload);
			return Ok(ApiResponse<int>.SuccessResponse());
        }

        [HttpPut("{id}/project-note")]
        public async Task<IActionResult> SaveProjectNote(Guid id, ProjectNoteModel payload)
        {
            var result = await projectsService.SaveProjectNote(payload);
            return Ok(ApiResponse<ProjectNoteDto>.SuccessResponse(result));
        }

        #endregion

        #region PATCH
        [HttpPatch("{id}/archive")]
		public async Task<IActionResult> ArchiveProject(Guid id)
		{
			await projectsService.ArchiveProjectAsync(id);
			return Ok(ApiResponse<int>.SuccessResponse());
		}

        [HttpPatch("{id}/project-journal")]
        public async Task<IActionResult> UpdateProjectJournal(Guid id, ProjectJournalPayload payload)
        {
			await projectJournalService.SaveAsync(id, payload, true);
            return Ok(ApiResponse<int>.SuccessResponse());
        }

        [HttpPatch("{id}/unarchive")]
        public async Task<IActionResult> UnArchiveProject(Guid id)
        {
            await projectsService.UnArchiveProjectAsync(id);
            return Ok(ApiResponse<int>.SuccessResponse());
        }

		[ApiExplorerSettings(IgnoreApi = true)]
		[HttpPatch("{id}/photo")]
		[Consumes("multipart/form-data")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UploadPhoto([FromForm] IFormFile file, [FromForm] Guid projectId, [FromForm] string projectName)
		{
			var directoryName = $"{projectName}/photo";
			var fileUrl = await azureStorageService.UploadFile(file, "project-settings", file.FileName, directoryName);
			await projectsService.UpdateProjectPhotoUrl(projectId, fileUrl);

			return Ok(ApiResponse<string>.SuccessResponse(fileUrl));
		}
		#endregion

		#region DELETE
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteProject(Guid id)
		{
			await projectsService.DeleteProjectAsync(id);
			return Ok(ApiResponse<int>.SuccessResponse());
		}
		#endregion
	}
}
