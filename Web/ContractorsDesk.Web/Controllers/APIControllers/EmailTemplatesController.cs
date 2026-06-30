using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/email-template")]
	[ApiController]
	public class EmailTemplatesController : BaseApiController
	{
		private readonly IEmailTemplateService emailTemplateService;

		public EmailTemplatesController(IEmailTemplateService emailTemplateService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.emailTemplateService = emailTemplateService;
		}

		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEmailTemplates()
		{
			List<EmailTemplateDto> emailTemplates = new List<EmailTemplateDto>();

			emailTemplates = await emailTemplateService.GetEmailTemplates();

			return Ok(ApiResponse<List<EmailTemplateDto>>.SuccessResponse(emailTemplates));
		}

		[HttpPost()]
		[ProducesResponseType(typeof(ApiResponse<EmailTemplateDto>), StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveEmailTemplate([FromBody] EmailTemplatePayload model)
		{
			var result = await emailTemplateService.SaveEmailTemplate(model);
			return Ok(ApiResponse<EmailTemplateDto>.SuccessResponse(result));
		}
	}
}
