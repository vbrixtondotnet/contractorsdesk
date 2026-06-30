using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[Route("Archives")]
    public class ArchivesController : BaseController
    {

        IActionItemsService actionItemService;
        IProjectsService projectsService;
        public ArchivesController(
			IActionItemsService actionItemService, 
			IProjectsService projectsService, 
			IServiceProvider provider) : base("Archives", provider) 
		{
			this.actionItemService = actionItemService;
            this.projectsService = projectsService;
		}

		[HttpGet]
		[Route("action-items")]
        public async Task<IActionResult> ActionItems(int id)
		{
			ViewBag.PageName = "Archived Action Items";
			ViewBag.Title = ViewBag.PageName;
			return RenderView();
        }

		[HttpGet]
		[Route("projects")]
		public async Task<IActionResult> Projects(int id)
		{
			ViewBag.PageName = "Archived Projects";
			ViewBag.Title = ViewBag.PageName;
			return RenderView();
		}

		[HttpGet]
		[Route("proposals")]
		public async Task<IActionResult> Proposals(int id)
		{
			ViewBag.PageName = "Archived Proposals";
			ViewBag.Title = ViewBag.PageName;
			return RenderView();
		}

	}
}
