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
	public class CustomerController : BaseApiController
	{
		private readonly ICustomerService customerService;
		public CustomerController(ICustomerService customerService, 
			IPermissionsService permissionsService, 
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, permissionsService, applicationUserService) 
		{
			this.customerService = customerService;
			this.customerService.UserId = this.UserId;
		}

		#region Get
		[HttpGet("customers")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SearchCustomers([FromQuery] string? key = "")
		{
			var response = await customerService.SearchCustomersAsync(key);

			return Ok(ApiResponse<List<ClientDto>>.SuccessResponse(response));
		}

		
        #endregion

       
	}
}
