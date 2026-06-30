using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class NotificationsController : BaseController
	{

		public NotificationsController(IServiceProvider provider) : base("Notifications", provider)
		{
		}


		[HttpGet]
		[Route("notifications")]
		public IActionResult Notifications()
		{
			ViewBag.PageName = "Notifications";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}
	}
}
