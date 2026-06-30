using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class SearchController : BaseController
    {
		public SearchController(IServiceProvider provider) : base("Search", provider) { }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string query)
		{
			ViewBag.PageName = "Search Results";
            ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
            ViewBag.Query = query;
			return RenderView();
        }
	}
}
