using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/dashboard")]
	[ApiController]
	public class DashboardController : BaseApiController
	{
		private readonly IActionItemsService actionItemsService;	
		private readonly IProposalService proposalService;
		private readonly IProjectsService projectsService;
		private readonly IDashboardService dashboardService;

		public DashboardController(
			IDashboardService dashboardService,
			IActionItemsService actionItemsService,
			IProjectsService projectsService,
			IProposalService proposalService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.actionItemsService = actionItemsService;
			this.proposalService = proposalService;
			this.projectsService = projectsService;
			this.dashboardService = dashboardService;


			this.actionItemsService.UserId = this.UserId;
			this.projectsService.UserId = this.UserId;
			this.proposalService.UserId = this.UserId;
			this.dashboardService.UserId = this.UserId;	
		}

		#region GET
		[HttpGet("action-items")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetDashboardActionItems([FromQuery] int? statusId = null, [FromQuery] Guid? projectId = null)
		{
			var response = await actionItemsService.GetDashboardActionItemsAsync(Permissions.CanManageAllActionItems, this.UserId, statusId, projectId);
			return Ok(ApiResponse<List<ActionItemSummaryViewDto>>.SuccessResponse(response));
		}

		[HttpGet("projects")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProjects(string? search = "")
		{
			var response = await projectsService.GetActiveJobsByUserAsync(this.CurrentRole.Value, this.UserId, this.Permissions.CanManageAllJobs);

			if (!string.IsNullOrEmpty(search))
			{
				response = response.Where(r => r.Name.ToLower().Contains(search.ToLower())).ToList();
			}

			return Ok(ApiResponse<List<ProjectDetailsDto>>.SuccessResponse(response));
		}

		[HttpGet("client-projects")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetClientProjects()
		{
			var response = await projectsService.GetClientProjects();
			return Ok(ApiResponse<List<ClientProjectDto>>.SuccessResponse(response));
		}

		[HttpGet("stats")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetStats()
		{
			var response = await dashboardService.GetDashboardStatsAsync(this.Permissions.CanManageAllJobs);
			return Ok(ApiResponse<DashboardStatsDto>.SuccessResponse(response));
		}
		#endregion
	}
}
