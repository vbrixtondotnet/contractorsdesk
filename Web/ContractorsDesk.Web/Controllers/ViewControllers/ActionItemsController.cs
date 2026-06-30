using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class ActionItemsController : BaseController
    {

        IActionItemsService actionItemService;
        IProjectsService projectsService;
        public ActionItemsController(
			IActionItemsService actionItemService, 
			IProjectsService projectsService, 
			IServiceProvider provider) : base("Dashboard", provider) 
		{
			this.actionItemService = actionItemService;
            this.projectsService = projectsService;
		}

		[HttpGet]
		[Route("action-items/{id}")]
        public async Task<IActionResult> ActionItem(int id)
		{
			ViewBag.PageName = "Action Item";
			return RenderView();
        }

	}
}
