using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class ProjectsController : BaseController
    {
		private readonly IProjectsService projectsService;
		public ProjectsController(IProjectsService projectsService, IServiceProvider provider) : base("Dashboard", provider) 
		{
			this.projectsService = projectsService;
		}

		[HttpGet]
		[Route("project/{id}")]
        public async Task<IActionResult> Project(Guid id, string t = "")
		{
			ViewBag.PageName = "Project Details";
			ViewBag.Title = ViewBag.PageName;
			ViewBag.Tab = t;

			return RenderView();
        }

		[HttpGet]
		[Route("project/{id}/schedule")]
		public async Task<IActionResult> Schedule(Guid id)
		{
			ViewBag.PageName = "Manage Schedule";
			ViewBag.Title = ViewBag.PageName;

			return RenderView();
		}

		private List<BreadcrumbItemViewModel> GenerateBreadcrumbs(Guid proposalId, Guid projectId, int proposalStatus)
		{
			return new List<BreadcrumbItemViewModel>
			{
				new BreadcrumbItemViewModel { Title = "Project Details", Url = $"/project/{projectId}" },
				new BreadcrumbItemViewModel { Title = "Proposal Details", Url = $"/proposals/{proposalId}" },
				new BreadcrumbItemViewModel { Title = "Schedule Details", Url = $"/project/{projectId}/schedule" },
				new BreadcrumbItemViewModel
				{
					Title = "Revise Estimates",
					Url = proposalStatus == 2 ? $"/revised-estimates/{proposalId}" : null
				}
			};
		}
	}
}
