using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[Route("client-contracts")]
	public class ClientContractsController : BaseController
    {
		public ClientContractsController(IServiceProvider provider) : base("Client Contracts", provider) { }

		[HttpGet]
        public IActionResult Index()
		{
			ViewBag.PageName = "Client Contracts";
			ViewBag.Action = "/";
			return RenderView();
        }
	}
}
