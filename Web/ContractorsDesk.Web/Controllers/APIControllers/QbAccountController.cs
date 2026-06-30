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
	public class QbAccountController : BaseApiController
	{
		private readonly IQbAccountService qbAccountService;
		public QbAccountController(IQbAccountService qbAccountService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.qbAccountService = qbAccountService;
			this.qbAccountService.UserId = this.UserId;
		}

		[HttpGet("qbaccounts")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEstimateDataMappings()
		{
			var response = await qbAccountService.GetQbAccountsAsync();
			return Ok(ApiResponse<List<QBAccountDto>>.SuccessResponse(response));
		}

		//[HttpPost("revised-estimates/{id}")]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//public async Task<IActionResult> SaveRevisedEstimates(Guid id, [FromBody] List<RevisedEstimateCategoryDto> categories)
		//{
		//	var response = await estimateService.SaveRevisedEstimateAsync(id, categories);
		//	return Ok(ApiResponse<bool>.SuccessResponse(response));
		//}

	}
}
