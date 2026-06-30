using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/reports")]
	[ApiController]
	public class ReportsController : BaseApiController
	{
		private readonly IReportsService reportsService;
		public ReportsController(
			IReportsService reportsService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.reportsService = reportsService;
			this.reportsService.UserId = this.UserId;
		}

		[HttpGet("active-jobs")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActiveJobs()
		{
			var response = await reportsService.GetActiveJobsSummaryAsync();
			return Ok(ApiResponse<List<JobsSummaryReportDto>>.SuccessResponse(response));
		}

		[HttpGet("pending-jobs")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetPendingJobs()
		{
			var response = await reportsService.GetPendingJobsSummaryAsync();
			return Ok(ApiResponse<List<JobsSummaryReportDto>>.SuccessResponse(response));
		}

		[HttpGet("active-construction-jobs")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ActiveConstructionJobs(DateOnly start, DateOnly end)
		{
			var response = await reportsService.GetActiveConstructionJobReportAsync(start, end, "All");
			return Ok(ApiResponse<List<ActiveConstructionJobSpResult>>.SuccessResponse(response));
		}

		[HttpGet("class-transactions")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ClassTransactions(DateOnly start, DateOnly end, string className, string filter)
		{
			var response = await reportsService.GetClassTransactionsReportAsync(start, end, className, filter);
			return Ok(ApiResponse<List<ClassTransactionsReportSpResult>>.SuccessResponse(response));
		}

		[HttpGet("transactions")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Transactions(DateOnly start, DateOnly end, string className, string filter)
		{
			var response = await reportsService.GetTransactionsReportAsync(start, end, className, filter);
			return Ok(ApiResponse<List<TransactionsReportSpResult>>.SuccessResponse(response));
		}
	}
}
