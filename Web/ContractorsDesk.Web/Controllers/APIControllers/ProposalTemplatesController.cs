using AutoMapper;
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
	[Route("api/proposal-templates")]
	[ApiController]
	public class ProposalTemplatesController : BaseApiController
	{
		private readonly IProposalTemplatesService proposalTemplatesService;
		public ProposalTemplatesController(IProposalTemplatesService proposalTemplatesService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.proposalTemplatesService = proposalTemplatesService;
			this.proposalTemplatesService.UserId = this.UserId;
		}

		#region GET

		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposalTemplates()
		{
			var response = await proposalTemplatesService.GetProposalTemplatesAsync();
			return Ok(ApiResponse<List<ProposalTemplateDto>>.SuccessResponse(response));
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await proposalTemplatesService.GetByIdAsync(id);
			return Ok(ApiResponse<ProposalTemplateDto>.SuccessResponse(response));
		}

		[HttpGet("{id}/line-items")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetLineItems(Guid id)
		{
			var response = await proposalTemplatesService.GetLineItemsAsync(id);
			return Ok(ApiResponse<List<ProposalTemplateLineItemDto>>.SuccessResponse(response));
		}

		[HttpGet("default")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposalTemplateUserDefault(Guid id)
		{
			var response = await proposalTemplatesService.GetProposalTemplateUserDefault();
			return Ok(ApiResponse<ProposalTemplateUserDefaultDto>.SuccessResponse(response));
		}

		#endregion

		#region POST

		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveProposalTemplate([FromBody] ProposalTemplatePayload payload)
		{
			var response = await proposalTemplatesService.CreateProposalTemplateAsync(payload);
			return Ok(ApiResponse<ProposalTemplateDto>.SuccessResponse(response));
		}

		[HttpPost("default/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveProposalTemplateUserDefault(Guid id)
		{
			await proposalTemplatesService.SaveProposalTemplateUserDefault(id);
			return Ok(ApiResponse<ProposalDto>.SuccessResponse());
		}
		#endregion

		#region PUT

		[HttpPut("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateProposalTemplate(Guid id, [FromBody] ProposalTemplatePayload payload)
		{
			payload.Id = id;
			var response = await proposalTemplatesService.UpdateProposalTemplateAsync(payload);
			return Ok(ApiResponse<ProposalTemplateDto>.SuccessResponse(response));
		}

        [HttpDelete("{id}")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await proposalTemplatesService.RemoveProposalTemplate(id);
			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}
        #endregion


        [HttpPatch("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeketeProposalTemplate(Guid id)
        {
            var response = await proposalTemplatesService.DeleteProposalTemplate(id);
            return Ok(ApiResponse<bool>.SuccessResponse(response));
        }

    }
}
