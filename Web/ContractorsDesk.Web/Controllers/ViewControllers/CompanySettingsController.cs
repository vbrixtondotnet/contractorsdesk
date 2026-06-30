using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class CompanySettingsController(IServiceProvider provider) : BaseController("CompanySettings", provider)
    {
		[HttpGet]
		[Route("company-settings")]
		public IActionResult CompanySettings()
		{
			ViewBag.PageName = "Company Settings";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}
	}
}
