using AutoMapper;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api")]
	[ApiController]
	public class TransactionsController : BaseApiController
	{
		private readonly ITransactionService transactionService;
		public TransactionsController(ITransactionService transactionService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.transactionService = transactionService;
			this.transactionService.UserId = this.UserId;
		}

		[HttpGet("transactions/{proposalId}/{estimateCategoryId}/{parentEstimateCategoryId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetTransactions(Guid proposalId, Guid? estimateCategoryId, Guid? parentEstimateCategoryId)
		{
			var response = await transactionService.GetTransactionDetailsAsync(proposalId, estimateCategoryId, parentEstimateCategoryId);
			return Ok(ApiResponse<List<TransactionDetailsDto>>.SuccessResponse(response));
		}
	}
}
