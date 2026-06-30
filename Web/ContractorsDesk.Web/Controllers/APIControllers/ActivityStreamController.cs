using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/activity-stream")]
	[ApiController]
	public class ActivityStreamController : BaseApiController
	{
		private readonly INotificationService notificationService;
		private readonly IActivityStreamService activityStreamService;

		public ActivityStreamController(
			IActivityStreamService activityStreamService,
			INotificationService notificationService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.notificationService = notificationService;
			this.activityStreamService = activityStreamService;
		}

		
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status202Accepted)]
		public async Task<IActionResult> CreateActionItem([FromBody] ActivityStreamPayload activityStreamPayload)
		{
			await activityStreamService.CreateActivityStream(activityStreamPayload);
			return Ok(ApiResponse<string>.SuccessResponse("Accepted"));
		}

		

		#region Private Methods
		#endregion
	}
}
