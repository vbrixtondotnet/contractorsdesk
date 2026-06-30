using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
    public class ProposalTemplatesController : BaseController
    {
        public ProposalTemplatesController(IServiceProvider provider) : base("Vendors", provider)
        {
        }


        [HttpGet]
		[Route("proposal-templates")]
        public async Task<IActionResult> ProposalTemplates()
		{
            ViewBag.Title = "Proposal Templates";
            ViewBag.PageName = ViewBag.Title;

            return RenderView();
        }

	}
}
