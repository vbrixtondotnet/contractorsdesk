using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class VendorsController : BaseController
    {
        public VendorsController(IServiceProvider provider) : base("Vendors", provider)
        {
        }


        [HttpGet]
		[Route("vendors")]
        public async Task<IActionResult> Vendors()
		{
            ViewBag.Title = "Vendors";
            ViewBag.PageName = ViewBag.Title;

            return RenderView();
        }

	}
}
