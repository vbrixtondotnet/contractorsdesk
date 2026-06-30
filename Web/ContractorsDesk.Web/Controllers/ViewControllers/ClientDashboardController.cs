using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class ClientDashboardController : BaseController
    {
        public ClientDashboardController(IServiceProvider provider) : base(string.Empty, provider)
        {
        }

        [Route("client-dashboard")]
		public IActionResult Dashboard()
		{
			ViewBag.Title = "My Projects";
			ViewBag.PageName = "My Projects";
			return RenderView();
        }
		[Route("client-project/{id}")]
		public async Task<IActionResult> Project(Guid id)
		{
			ViewBag.PageName = "Project";
			ViewBag.Title = ViewBag.PageName;

			return RenderView();
		}
	}
}
