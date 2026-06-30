using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[Route("users")]
    public class UsersController : BaseController
    {
		private readonly IApplicationUserService applicationUserService;
		public UsersController(IApplicationUserService applicationUserService, IServiceProvider provider) : base("Administrator", provider)
		{
			this.applicationUserService = applicationUserService;
		}

		[HttpGet]
		public IActionResult Users()
		{
			ViewBag.PageName = "Users";
			return RenderView();
		}


		[HttpGet]
		[Route("account")]
		public async Task<IActionResult> Account()
		{
			var accountDetails = await this.applicationUserService.GetAccountDetailsAsync(this.UserId);
			ViewBag.PageName = "My Account";
			return RenderView(accountDetails);
		}
	}
}
