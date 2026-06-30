using ContractorsDesk.Core.ApiPayloadModels;
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
	[Route("api/contracts")]
	[ApiController]
	public class ContractsController : BaseApiController
	{
		private readonly IContractService contractService;

		public ContractsController(IContractService contractService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.contractService = contractService;
		}

		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetContracts()
		{
			List<ContractDto> contracts = new List<ContractDto>();

			contracts = await contractService.GetContracts();

			return Ok(ApiResponse<List<ContractDto>>.SuccessResponse(contracts));
		}

		[HttpPost()]
		[ProducesResponseType(typeof(ApiResponse<ContractDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveContract([FromBody] ContractPayload model)
		{
			var result = await contractService.SaveContract(model);
			return Ok(ApiResponse<ContractDto>.SuccessResponse(result));
		}
	}
}
