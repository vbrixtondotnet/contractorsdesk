using Azure;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Core.Utilities;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	//[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/invoices")]
	[ApiController]
	public class InvoiceController : BaseApiController
	{
		private readonly IInvoiceService invoiceService;

		public InvoiceController(
			IInvoiceService invoiceService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.invoiceService = invoiceService;
			this.invoiceService.UserId = this.UserId;
		}

		#region GET
		[HttpGet("invoiceno")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActionItem([FromQuery] Guid clientId)
		{
			var response = await invoiceService.GenerateInvoiceNumberAsync(clientId);
			return Ok(ApiResponse<string>.SuccessResponse(response));
		}
		#endregion

		#region POST
		[HttpPost]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		[ApiExplorerSettings(IgnoreApi = true)]
		public async Task<IActionResult> CreateInvoiceAsync([FromBody] InvoicePayload payload)
		{
			var invoice = await invoiceService.CreateInvoiceAsync(payload);

			return Ok(ApiResponse<InvoiceDto>.SuccessResponse(invoice));
		}
		#endregion
	}
}
