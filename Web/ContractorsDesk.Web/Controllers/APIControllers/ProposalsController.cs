using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/proposals")]
	[ApiController]
	public class ProposalsController : BaseApiController
	{
		private readonly IProposalService proposalService; 
		private readonly IProjectsService projectsService;
		private readonly IClientDocumentService clientDocumentService;


		public ProposalsController(
			IProposalService proposalService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IClientDocumentService clientDocumentService,
			IHttpContextAccessor httpContextAccessor,
			IAzureStorageService azureStorageService,
			IProjectsService projectsService) : 
			base(httpContextAccessor, permissionsService, applicationUserService, azureStorageService)
		{
			this.proposalService = proposalService;
			this.proposalService.UserId = this.UserId;
			this.projectsService = projectsService;
			this.clientDocumentService = clientDocumentService;
		}

		#region GET
		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposals([FromQuery] ProposalsQueryParameter p)
		{
			int? supervisorId = this.Permissions.CanManageAllEstimates ? null : UserId;

			var response = await proposalService.GetProposalsAsync(supervisorId, p.Status == "archived");
			return Ok(ApiResponse<List<ProposalManagementDto>>.SuccessResponse(response));
		}

		[HttpGet("templates/{id}/items/{name}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposalTemplateLineItems(Guid id, string name)
		{
			var response = await proposalService.GetLineItemsByNameAndTemplateAsync(id, name);
			return Ok(ApiResponse<List<ProposalTemplateLineItemDto>>.SuccessResponse(response));
		}

		[HttpGet("{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposal(Guid id)
		{
			var response = id == Guid.Empty ? await proposalService.NewProposalAsync() : await proposalService.GetProposalAsync(id);
			return Ok(ApiResponse<ProposalDto>.SuccessResponse(response));
		}

		[HttpGet("{id}/details")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposalDetails(Guid id)
		{
			var response = await proposalService.GetProposalDetailsAsync(id);
			return Ok(ApiResponse<ProposalDto>.SuccessResponse(response));
		}

		[HttpGet("project")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetActiveProject([FromQuery] string name)
		{
			var response = await proposalService.CheckActiveProjectAsync(name);

			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}

		[HttpGet("{id}/line-item")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> IsLineItemAdded(Guid id, [FromQuery] string name)
		{
			var response = await proposalService.IsLineItemAdded(id, name);
			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}

		[HttpGet("{id}/lines")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetProposalLines(Guid id)
		{
			var response = await proposalService.GetProposalLinesAsync(id);
			return Ok(ApiResponse<List<ProposalLineDto>>.SuccessResponse(response));
		}

        [HttpGet("{id}/download-csv")]
        public async Task<IActionResult> DownloadProposalCsv(Guid id)
        {
			var proposal = await proposalService.GetProposalAsync(id);
            var lines = await proposalService.GetProposalLinesForExportGroupedAsync(id);

            var csvLines = new List<string>
			{
				"Sequence,Name,Description,Amount"
			};

            foreach (var p in lines)
            {
                var sequence = p.Sequence > 0 ? p.Sequence.ToString() : "";
                var line = $"{sequence},\"{p.Name?.Replace("\"", "\"\"")}\",\"{p.Description?.Replace("\"", "\"\"")}\",{p.Amount}";
                csvLines.Add(line);
            }

            var csvContent = string.Join(Environment.NewLine, csvLines);
            var bytes = Encoding.UTF8.GetBytes(csvContent);

            return File(bytes, "text/csv", $"{proposal.Project?.Name}.csv");
        }
		#endregion

		#region POST

		[HttpPost("validate-client-email")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> ValidateEmailAddress([FromBody] ClientModel model)
		{
			var validationResult = await proposalService.ValidateClientEmailAddress(model);

			return Ok(ApiResponse<bool>.SuccessResponse(validationResult));
		}

		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveProposal([FromBody] ProposalModel model)
		{
			//proposalDto.Supervisors = this.CurrentRole != Roles.ProjectManager ? model.Supervisors : new List<int> { this.UserId };
			var retval = new SaveProposalResponseDto();

			var matchingProjects = await proposalService.GetMatchingProjectsByNameAsync(model.Project.Name);
			var matchingDraftProposals = await proposalService.GetMatchingDraftProposalsByNameAsync(model.Project.Name);

			if (matchingProjects == null && matchingDraftProposals == null)
            {
                retval.Proposal = await proposalService.SaveProposalAsync(model);
            }

			retval.MatchingProjects = matchingProjects;
			retval.MatchingDraftProposals = matchingDraftProposals;

			return Ok(ApiResponse<SaveProposalResponseDto>.SuccessResponse(retval));
		}

		[HttpPost("save-as-new")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveAsNewProposal([FromBody] ProposalModel model)
		{
			var retval = new SaveProposalResponseDto();
			retval.Proposal = await proposalService.SaveProposalAsync(model);
			return Ok(ApiResponse<SaveProposalResponseDto>.SuccessResponse(retval));
		}

		[HttpPost("save-and-merge")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveAndMergeProposal([FromBody] ProposalModel model)
		{
			var retval = new SaveProposalResponseDto();
			retval.Proposal = await proposalService.SaveProposalAsync(model, merge: true);
			return Ok(ApiResponse<SaveProposalResponseDto>.SuccessResponse(retval));
		}
		[HttpPost("save-and-overwrite")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveAndOverwriteProposal([FromBody] ProposalModel model)
		{
			var retval = new SaveProposalResponseDto();
			retval.Proposal = await proposalService.SaveProposalAsync(model, overwrite:true);
			return Ok(ApiResponse<SaveProposalResponseDto>.SuccessResponse(retval));
		}

		[HttpPost("{id}/send-contract")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SendClientContractPDF(Guid id, [FromBody]ClientContractDetails clientContractDetails)
		{
			var documentId = await clientDocumentService.SendCostPlusDocument(id, clientContractDetails);
			return Ok(ApiResponse<object>.SuccessResponse(new { documentId }));
		}

		#endregion

		#region PUT

		[HttpPut()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateProposal([FromBody] ProposalModel model)
		{
			var retval = new SaveProposalResponseDto();

			List<ProjectMatchResultDto>? matchingProjects = null;
			List<ProjectMatchResultDto>? matchingDraftProposals = null;

			if (model.Project != null)
			{
				matchingProjects = await proposalService.GetMatchingProjectsByNameAsync(model.Project.Name, model.QbClassId);
				matchingDraftProposals = await proposalService.GetMatchingDraftProposalsByNameAsync(model.Project.Name, model.Id);
			}

			if (matchingProjects == null && matchingDraftProposals == null)
			{
				retval.Proposal = await proposalService.SaveProposalAsync(model);
			}

			retval.MatchingProjects = matchingProjects;
			retval.MatchingDraftProposals = matchingDraftProposals;

			return Ok(ApiResponse<SaveProposalResponseDto>.SuccessResponse(retval));

		}
		#endregion

		#region PATCH

		[HttpPatch("{id}/archive")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> ArchiveProposal(Guid id)
		{
			var response = await proposalService.ArchiveProposal(id);
			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}

		[HttpPatch("{id}/unarchive")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> UnArchiveProposal(Guid id)
		{
			var response = await proposalService.ArchiveProposal(id, false);
			return Ok(ApiResponse<bool>.SuccessResponse(response));
		}

		[HttpPatch("{id}/status")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> UpdateProposalStatus([FromBody] UpdateProposalStatusPayload payload, Guid id)
		{
			payload.ProposalId = id;
			var proposal = await proposalService.UpdateStatusAsync(payload);
			if(proposal.QbclassId != null)
			{
				var project = await projectsService.GetProjectShortDetailsAsync(proposal.QbclassId.Value);
				return Ok(ApiResponse<ProjectDetailsDto>.SuccessResponse(project));
			}
			else
			{
				return Ok(ApiResponse<ProjectDetailsDto>.SuccessResponse());
			}
		}

        [HttpPatch("{id}/include-zero-amount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProposalIncludeZeroAmount(Guid id)
        {
            await proposalService.UpdateProposalIncludeZeroAmount(id);
            return Ok(ApiResponse<ProposalDto>.SuccessResponse());
        }

        #endregion

        #region DELETE

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProposal(Guid id)
        {
            var response = await proposalService.DeleteProposal(id);
            return Ok(ApiResponse<bool>.SuccessResponse(response));
        }
        #endregion
    }
}
