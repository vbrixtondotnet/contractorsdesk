using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class EmailTemplatesController : BaseController
	{
		public EmailTemplatesController(IServiceProvider provider)
			: base("EmailTemplates", provider)
		{ }

		[HttpGet]
		[Route("email-templates")]
		public IActionResult EmailTemplates()
		{
			ViewBag.Title = "Email Templates";
			ViewBag.PageName = ViewBag.Title;
			return RenderView();
		}
	}
}
