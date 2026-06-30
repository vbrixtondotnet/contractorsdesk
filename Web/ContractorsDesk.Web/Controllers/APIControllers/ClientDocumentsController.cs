using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/client-documents")]
	[ApiController]
	public class ClientDocumentsController : BaseApiController
	{
		private readonly IClientDocumentService clientDocumentService;
		public ClientDocumentsController(
			IClientDocumentService clientDocumentService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.clientDocumentService = clientDocumentService;	
		}

		[HttpGet("{projectId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetClientDocuments(Guid projectId)
		{
			var clientDocuments = await this.clientDocumentService.GetClientFoldersAsync(projectId);
			return Ok(ApiResponse<List<SysFolderDto>>.SuccessResponse(clientDocuments));
		}

		[HttpGet("{projectId}/{folderId}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetClientDocument(Guid projectId, Guid folderId)
		{
			var clientDocuments = await this.clientDocumentService.GetClientDocumentsAsync(projectId, folderId);
			return Ok(ApiResponse<SysFolderDto>.SuccessResponse(clientDocuments));
		}

		[HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> DeleteClientDocument(Guid id)
		{
			var result = await this.clientDocumentService.DeleteClientDocument(id);
			return Ok(ApiResponse<bool>.SuccessResponse(result));
		}
	}
}
