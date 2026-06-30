using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class HomeController : BaseController
    {
        public HomeController(IServiceProvider provider) : base(string.Empty, provider)
        {
        }

        //[Route("dashboard")]
		public IActionResult Dashboard()
		{
			if (this.CurrentRole == Roles.SuperAdmin)
			{
				return Redirect("/users");
			}
			else if (this.CurrentRole == Roles.Client)
			{
				return Redirect("/client-dashboard");
			}

			ViewBag.Title = "Dashboard";
			ViewBag.PageName = "Dashboard";
			return RenderView();
        }

		[Route("dashboard-new")]
		public IActionResult DashboardNew()
		{
			if (this.CurrentRole == Roles.SuperAdmin)
			{
				return Redirect("/users");
			}

			ViewBag.Title = "Dashboard";
			ViewBag.PageName = "Dashboard";
			return RenderView();
		}

	}
}
