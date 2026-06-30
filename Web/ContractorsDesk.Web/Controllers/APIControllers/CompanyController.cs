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
	[Route("api/company")]
	[ApiController]
	public class CompanyController : BaseApiController
	{
		private readonly ICompanySettingService companySettingService;
		public CompanyController(
			ICompanySettingService companySettingService,
			IHttpContextAccessor httpContextAccessor,
            IPermissionsService permissionsService,
			IApplicationUserService applicationUserService) 
			: base(httpContextAccessor, permissionsService, applicationUserService)
        {
			this.companySettingService = companySettingService;
		}

		[HttpGet("users")]
		public async Task<IActionResult> GetCompanyUsers()
		{
			var companyId = this.CompanyId;
			var companyUsers = await applicationUserService.GetUsersByCompanyAsync(companyId);
			return Ok(ApiResponse<List<ApplicationUserDto>>.SuccessResponse(companyUsers));
		}

		[HttpGet("settings")]
		public async Task<IActionResult> GetCompanySettings()
		{
			var companySettings = await companySettingService.GetCompanySettingAsync();
			return Ok(ApiResponse<CompanySettingDto>.SuccessResponse(companySettings));
		}

        [HttpPost("settings")]
        public async Task<IActionResult> SaveCompanySettings(CompanySettingPayload companySettingPayload)
        {
            var companySettings = await companySettingService.UpdateCompanySettingAsync(companySettingPayload);
            return Ok(ApiResponse<CompanySettingDto>.SuccessResponse(companySettings));
        }
    }
}
