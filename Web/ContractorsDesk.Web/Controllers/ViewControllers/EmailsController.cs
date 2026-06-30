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
    public class EmailsController(IServiceProvider provider) : BaseController("Emails", provider)
    {
		[HttpGet]
		[Route("emails")]
		public IActionResult Inbox()
		{
			ViewBag.Title = "Inbox";
			ViewBag.PageName = "Inbox";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}

		[HttpGet]
		[Route("sent-items")]
		public IActionResult SentItems()
		{
			ViewBag.Title = "Sent Items";
			ViewBag.PageName = "Sent Items";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}
	}
}
