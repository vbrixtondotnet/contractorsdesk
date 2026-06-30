using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractorsDesk.WebPortal.Controllers.ViewControllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]
	public class ProposalsController : BaseController
	{
		private readonly IPDFService pDFService;
        private readonly IProposalService proposalService;
		public ProposalsController(IPDFService pDFService, IProposalService proposalService, IServiceProvider provider)
			: base("Proposals", provider)
		{
			this.pDFService = pDFService;
			this.proposalService = proposalService;
		}

		[HttpGet]
		[Route("proposals")]
		public IActionResult ProposalManagement(string id)
		{
			ViewBag.PageName = "Proposals";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Proposals", Url = "/proposals" };
			return RenderView();
		}

		[HttpGet]
		[Route("proposals/{id}")]
		public async Task<IActionResult> Proposal(Guid id)
		{
			ViewBag.Title = id != Guid.Empty ? "View Proposal" : "New Proposal";
			ViewBag.PageName = ViewBag.Title;
			return RenderView();
		}

		[HttpGet]
		[Route("proposals/templates/{id}")]
		public IActionResult ProposalTemplates(string id)
		{
			ViewBag.PageName = "Proposal Templates";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Proposals", Url = "/proposals" };
			return RenderView();
		}

		[HttpGet]
		[Route("proposals/revise-estimate/{id}")]
		public IActionResult ReviseEstimate(string id)
		{
			ViewBag.PageName = "Revised Estimate";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Proposals", Url = "/proposals" };
			return RenderView();
		}

		[HttpGet]
		[Route("templates")]
		public IActionResult Templates()
		{
			ViewBag.PageName = "Templates";
			ViewBag.PrevPage = new BreadcrumbItemViewModel { Title = "Dashboard", Url = "/" };
			return RenderView();
		}
		private List<BreadcrumbItemViewModel> GenerateBreadcrumbs(Guid proposalId, Guid projectId, int proposalStatus)
		{
			return new List<BreadcrumbItemViewModel>
			{
				new BreadcrumbItemViewModel { Title = "Project Details", Url = $"/project/{projectId}" },
				new BreadcrumbItemViewModel { Title = "Proposal Details", Url = $"/proposals/{proposalId}" },
				new BreadcrumbItemViewModel { Title = "Schedule Details", Url = $"/project/{projectId}/schedule" },
				new BreadcrumbItemViewModel
				{
					Title = "Revise Estimates",
					Url = proposalStatus == 2 ? $"/revised-estimates/{proposalId}" : null
				}
			};
		}
	}
}
