using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
    [Authorize]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Route("api/quickbooks")]
    [ApiController]
    public class QuickBooksController : BaseApiController
    {
        private readonly IQuickBooksService quickBooksService;
        public QuickBooksController(
            IQuickBooksService quickBooksService,
            IHttpContextAccessor httpContextAccessor,
            IPermissionsService permissionsService,
            IApplicationUserService applicationUserService)
            : base(httpContextAccessor, permissionsService, applicationUserService)
        {
            this.quickBooksService = quickBooksService;
        }

        [HttpGet]
        public async Task<IActionResult> HasQuickBooksAccountConnected()
        {
            var hasQuickBooksAccountConnected = await quickBooksService.HasQuickBooksAccountConnected();
            return Ok(ApiResponse<bool>.SuccessResponse(hasQuickBooksAccountConnected));
        }

        [HttpGet("reports/profitandloss")]
        public async Task<IActionResult> GetProfitAndLossReport([FromQuery] string classListIds)
		{
			var classListIdsList = classListIds.Split(',').Select(long.Parse).ToList();
            
			var profitAndLossReport = await quickBooksService.GetProfitAndLossReportAsync(classListIdsList);
			return Ok(ApiResponse<object>.SuccessResponse(profitAndLossReport));
		}
    }
}
