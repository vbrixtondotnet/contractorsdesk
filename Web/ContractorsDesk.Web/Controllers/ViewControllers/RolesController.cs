using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class RolesController : BaseController
    {
		public RolesController(IServiceProvider provider) : base("Administrator", provider){}
		[HttpGet]
		[Route("roles")]
        public IActionResult Roles()
		{
			ViewBag.PageName = "Roles";
			return RenderView();
        }

		[HttpGet]
		[Route("roles/{id}")]
		public IActionResult ViewRole(int id)
		{
			
			ViewBag.PageName = "View Role";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Roles", Url = "/roles" }; 
			return RenderView();
		}

		[HttpGet]
		[Route("roles/{id}/edit")]
		public IActionResult EditRole(int id)
		{
			ViewBag.PageName = "Edit Role";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Roles", Url = "/roles" };
			return RenderView();
		}

	}
}
