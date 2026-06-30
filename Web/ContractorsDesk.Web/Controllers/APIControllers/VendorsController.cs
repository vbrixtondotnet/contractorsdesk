using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/vendors")]
	[ApiController]
	public class VendorsController : BaseApiController
	{
		private readonly IVendorService vendorService;
		public VendorsController(IVendorService vendorService, 
			IPermissionsService permissionsService, 
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, permissionsService, applicationUserService) 
		{
			this.vendorService = vendorService;
		}

		#region Get
		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetVendors([FromQuery] string? key = null, [FromQuery] Guid? projectId = null, [FromQuery] bool excludeExisting = false)
		{
			List<VendorDto> vendors = new List<VendorDto>();

			if (projectId == null || projectId == Guid.Empty)
			{
                vendors = await vendorService.GetVendorsAsync(key);
			}

			return Ok(ApiResponse<List<VendorDto>>.SuccessResponse(vendors));
		}

        [HttpGet("types")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubcontractorTypes()
        {
            var result = await vendorService.GetVendorTypesAsync();
            return Ok(ApiResponse<List<string>>.SuccessResponse(result));
        }

        [HttpPost()]
        [ProducesResponseType(typeof(ApiResponse<VendorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddVendor([FromBody] VendorPayload model)
        {
            var result = await vendorService.AddVendorAsync(model);
            return Ok(ApiResponse<VendorDto>.SuccessResponse(result));
        }

        [HttpPatch("delete/{id}")]
        [ProducesResponseType(typeof(ApiResponse<VendorDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteVendor([FromRoute] Guid id)
        {
            var result = await vendorService.DeleteVendorAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }
        #endregion


    }
}
