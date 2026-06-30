using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]

	[Route("settings")]
	public class SettingsController : BaseController
	{
		public SettingsController(IServiceProvider provider) : base("Settings", provider)
		{
		}

		[HttpGet("estimate-data-mapping")]
		public IActionResult EstimateDataMapping()
		{
			ViewBag.PageName = "Estimate Data Mapping";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}

		[HttpGet("schedule-data-mapping")]
		public IActionResult ScheduleDataMapping()
		{
			ViewBag.PageName = "Schedule Tasks Mapping";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}

		[HttpGet("estimate-categories")]
		public IActionResult EstimateCategories()
		{
			ViewBag.PageName = "Estimate Categories";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}

		[HttpGet("construction-tasks")]
		public IActionResult ConstructionTasks()
		{
			ViewBag.PageName = "Construction Tasks";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}

		[HttpGet]
		public IActionResult Settings()
		{
			ViewBag.PageName = "Company Settings";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}

		//[HttpGet("sync-from-live")]
		//public async Task<IActionResult> SynchTestDbFromLive()
		//{
		//	ViewBag.PageName = "Sync Test DB From Live";
		//	await this.prodToTestDbSyncService.RunSyncDataFromProductionToTestAsync();
		//	return Redirect("/settings");
		//}
	}
}
