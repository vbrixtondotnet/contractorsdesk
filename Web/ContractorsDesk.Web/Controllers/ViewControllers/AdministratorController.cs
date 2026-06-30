using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class AdministratorController : BaseController
    {	
		[HttpGet]
		[Route("permissions")]
        public IActionResult Permissions()
		{
			ViewBag.PageName = "Permissions";
			return RenderView();
        }
    }
}
