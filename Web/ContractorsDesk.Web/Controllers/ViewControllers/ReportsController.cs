using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	[Route("reports")]
	public class ReportsController : BaseController
	{
		private readonly IQbClassService qbClassService;
		public ReportsController(IQbClassService qbClassService, 
			IServiceProvider provider,
			IReportsService reportsService) : base("Reports", provider)
		{
			this.qbClassService = qbClassService;
		}

		[Route("jobs-summary")]
		public IActionResult JobSummary()
		{
			ViewBag.PageName = "Jobs Summary Report";
			return RenderView();
		}

		[Route("profit-and-loss")]
		public async Task<IActionResult> ProfitAndLoss()
		{
			// Get the list of active classes
			var activeClasses = await qbClassService.GetActiveQbClassesAsync();
			var vm = new ProfitAndLossReportViewModel
			{
				ClassList = activeClasses,
			};

			ViewBag.PageName = "Profit and Loss Report";
			return RenderView(vm);
		}

		[Route("budget-to-actual-cost")]
		public IActionResult BudgetToActualCost()
		{
			ViewBag.PageName = "Budget To Actual Cost Report";
			return RenderView();
		}

		[Route("proposal")]
		public IActionResult Proposal()
		{
			ViewBag.PageName = "Proposal Report";
			return RenderView();
		}

		[Route("schedule")]
		public IActionResult Schedule()
		{
			ViewBag.PageName = "Schedule Report";
			return RenderView();
		}

        [Route("updated-budget")]
        public IActionResult ChangeOrder()
        {
            ViewBag.PageName = "Updated Budget Report";
            return RenderView();
        }

		[Route("active-construction-jobs")]
		public IActionResult ActiveConstructionJobs()
		{
			ViewBag.Title = "Active Construction Jobs";
			ViewBag.PageName = ViewBag.Title;
			return RenderView();
		}

		[Route("class-transactions")]
		public IActionResult ClassTransactions()
		{
			ViewBag.Title = "Class Transactions Report";
			ViewBag.PageName = ViewBag.Title;
			return RenderView();
		}

		[Route("transactions")]
		public IActionResult Transactions()
		{
			ViewBag.Title = "Transactions Report";
			ViewBag.PageName = ViewBag.Title;
			return RenderView();
		}
	}
}
