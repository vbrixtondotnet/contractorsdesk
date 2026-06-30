using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class SubContractorsController : BaseController
	{
		public SubContractorsController(IServiceProvider provider) : base("Sub-Contractors", provider) 
		{ 
		}

		[HttpGet]
		[Route("subcontractors")]
		public async Task<IActionResult> SubContractors(Guid id)
		{
			ViewBag.Title = "Sub-Contractors";
			ViewBag.PageName = ViewBag.Title;

			return RenderView();
		}
	}
}
