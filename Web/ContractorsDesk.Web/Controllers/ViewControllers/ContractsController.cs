using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class ContractsController : BaseController
	{
		public ContractsController(IServiceProvider provider)
			: base("Contracts", provider)
		{ }

		[HttpGet]
		[Route("contracts")]
		public IActionResult Contracts()
		{
			ViewBag.Title = "Contracts";
			ViewBag.PageName = ViewBag.Title;
			return RenderView();
		}
	}
}
