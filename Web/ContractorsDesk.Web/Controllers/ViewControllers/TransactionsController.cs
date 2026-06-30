using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class TransactionsController : BaseController
	{
		private readonly ITransactionService transactionService;
		private readonly IProposalService proposalService;
		public TransactionsController(
			ITransactionService transactionService,
			IProposalService proposalService,
			IServiceProvider provider) : base("Proposals", provider)
		{
			this.transactionService = transactionService;
			this.proposalService = proposalService;
		}


		[HttpGet("transaction-detail-report/{proposalId}/{estimateCategoryId}/{parentEstimateCategoryId}")]
		public async Task<IActionResult> TransactionDetailReport(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId)
		{
			ViewBag.PageName = "Transaction Detail Report";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Revised Estimate", Url = $"/revised-estimates/{proposalId}" };
			
			var transactions = await transactionService.GetTransactionDetailsAsync(proposalId, estimateCategoryId, parentEstimateCategoryId);	
			var proposalDetails = await proposalService.GetProposalDetailsAsync(proposalId);

			var projectId = proposalDetails.Project != null ? proposalDetails.Project.Id : Guid.Empty;
			var isOwnerDepositsReport = parentEstimateCategoryId == Guid.Parse("36fde9f1-dce7-4c5b-8fb8-4c23e1fe3f38");

			var totalCost = isOwnerDepositsReport ? await transactionService.GetTotalCost(projectId) : 0;

			var vm = new TransactionDetailsViewModel
			{
				ItemName = string.Empty,
				TransactionDetails = transactions,
				ProjectName = proposalDetails.Project?.Name,
				IsOwnerDepositsReport = isOwnerDepositsReport, 
				TotalCost = totalCost
			};
			
			return RenderView(vm);
		}
	}
}
