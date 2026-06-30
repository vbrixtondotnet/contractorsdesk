using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/print")]
	[ApiController]
	public class PrintController : BaseApiController
	{
		private readonly IProposalService proposalService;
		private readonly IProjectsService projectsService;	
		private readonly IPDFService pDFService;
		private readonly IScheduleService scheduleService;
		public PrintController(
			IProposalService proposalService,
			IProjectsService projectsService,
			IPDFService pDFService,
			IScheduleService scheduleService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.proposalService = proposalService;
			this.pDFService = pDFService;
			this.scheduleService = scheduleService;
			this.projectsService = projectsService;
			proposalService.UserId = this.UserId;
		}

		[HttpGet("proposal/{id}")]
		public async Task<IActionResult> GetProposalPDF(Guid id)
		{
            var fullUrl = GetFullUrl($"pdf/proposal/{id}");
			var proposal = await proposalService.GetProposalAsync(id);

			if(proposal == null) throw new Exception("Proposal not found");

			var fileName = $"{proposal.Project.Name.Replace(" ", "")}-Cost-Estimate.pdf";

			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("revised-estimate/{id}")]
		public async Task<IActionResult> GetRevisedEstimatePDF(Guid id)
		{
			var fullUrl = GetFullUrl($"pdf/revised-estimate/{id}");
			var proposal = await proposalService.GetProposalAsync(id);
			var projectName = proposal.Project.Name;
			var fileName = $"{projectName.Replace(" ", "")}-Revised-Estimate.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("project-schedule/{id}")]
		public async Task<IActionResult> GetProjectSchedulePDF(Guid id)
		{
			var fullUrl = GetFullUrl($"pdf/project-schedule/{id}");
			var projectSchedule = await scheduleService.GetProjectScheduleAsync(id);
			var projectName = projectSchedule.ProjectDetails.Name;
			var fileName = $"{projectName.Replace(" ", "")}-Project-Schedule.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("transaction-details")]
		public async Task<IActionResult> GetProjectSchedulePDF(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId)
		{
			var fullUrl = GetFullUrl($"pdf/transaction-details?proposalId={proposalId}&estimateCategoryId={estimateCategoryId}&parentEstimateCategoryId={parentEstimateCategoryId}");
			var proposal = await proposalService.GetProposalAsync(proposalId);
			var projectName = proposal.Project.Name;
			var fileName = $"{projectName.Replace(" ", "")}-Transaction-Details-Report.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("cost-change/{actionItemId}")]
		public async Task<IActionResult> GetCostChangePDF(int actionItemId)
		{
			var fullUrl = GetFullUrl($"pdf/cost-change/{actionItemId}");			
			var fileName = $"{DateTime.UtcNow.ToShortDateString()}-Cost-Change-Form.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("invoice/{invoiceId}")]
		public async Task<IActionResult> GetInvoicePdf(Guid invoiceId)
		{
			var fullUrl = GetFullUrl($"pdf/invoice/{invoiceId}");
			var fileName = $"{DateTime.UtcNow.ToShortDateString()}-Invoice.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("status-report/{id}")]
		public async Task<IActionResult> GetStatusReportPDF(Guid id)
		{
			var fullUrl = GetFullUrl($"pdf/status-report/{id}");
			var fileName = $"{DateTime.UtcNow.ToShortDateString()}-status-report.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("change-order/{id}/project")]
		public async Task<IActionResult> ChangeOrderProjectPdf(Guid id)
		{
            var fullUrl = GetFullUrl($"pdf/change-order/{id}/project");
            var proposal = await proposalService.GetProposalAsync(id);
            var projectName = proposal.Project.Name;
            var fileName = $"{projectName.Replace(" ", "")}-cost-revision-report.pdf";
            var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

            return File(pdfStream, "application/pdf", fileName);
        }

		[HttpGet("active-construction-jobs")]
		public async Task<IActionResult> ActiveConstructionJobs(DateOnly start, DateOnly end)
		{
			var fullUrl = GetFullUrl($"pdf/active-construction-jobs?start={start}&end={end}");
			var fileName = $"active-construction-jobs.{start}-{end}.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("class-transactions")]
		public async Task<IActionResult> ClassTransactions(DateOnly start, DateOnly end, string className, string filter)
		{
			var fullUrl = GetFullUrl($"pdf/class-transactions?start={start}&end={end}&className={className}&filter={filter}");
			var fileName = $"class-transactions.{start}-{end}.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl, DinkToPdf.Orientation.Landscape);

			return File(pdfStream, "application/pdf", fileName);
		}

		[HttpGet("transactions")]
		public async Task<IActionResult> Transactions(DateOnly start, DateOnly end, string className, string filter)
		{
			var fullUrl = GetFullUrl($"pdf/transactions?start={start}&end={end}&className={className}&filter={filter}");
			var fileName = $"transactions.{start}-{end}.pdf";
			var pdfStream = await this.pDFService.ConvertHtmlToPdfFromUrl(fullUrl, DinkToPdf.Orientation.Landscape);

			return File(pdfStream, "application/pdf", fileName);
		}

		[ApiExplorerSettings(IgnoreApi = true)]
		private string GetFullUrl(string path)
		{
			var request = HttpContext.Request;
			var rootUrl = $"{request.Scheme}://{request.Host}";
			return $"{rootUrl}/{path}";
		}

	}
}
