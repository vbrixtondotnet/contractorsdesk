using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignNow.Net.Interfaces;
using System.Net.Mime;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/signnow")]
	[ApiController]
	public class SignNowController : BaseApiController
	{
		private readonly ISignNowService signNowService;
		public SignNowController(ISignNowService signNowService,
            IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.signNowService = signNowService;
		}

		//[HttpGet("document/{id}/fields")]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//public async Task<IActionResult> GetActionItem(string id)
		//{
		//	var response = await signNowService.GetDocumentFieldsAsync(id);
		//	return Ok(ApiResponse<List<ISignNowField>>.SuccessResponse(response));
		//}

	}
}
