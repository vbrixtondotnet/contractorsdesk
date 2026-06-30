using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class EstimatesController : BaseController
	{
		private readonly IProjectsService projectsService;
		public EstimatesController(IProjectsService projectsService, IServiceProvider provider) : base("Proposals", provider) 
		{ 
			this.projectsService = projectsService;
		}

		[HttpGet]
		[Route("revised-estimates/{id}")]
		public async Task<IActionResult> RevisedEstimates(Guid id)
		{
			ViewBag.Title = $"Revise Estimate";
			ViewBag.PageName = ViewBag.Title;

			return RenderView();
		}
	}
}
