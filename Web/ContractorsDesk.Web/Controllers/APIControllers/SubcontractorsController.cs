using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services;
using ContractorsDesk.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Net.Mime;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/sub-contractors")]
	[ApiController]
	public class SubcontractorsController : BaseApiController
	{
		private readonly ISubcontractorsService subcontractorsService;
		public SubcontractorsController(ISubcontractorsService subcontractorsService, 
			IPermissionsService permissionsService, 
			IApplicationUserService applicationUserService,
			IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor, permissionsService, applicationUserService) 
		{
			this.subcontractorsService = subcontractorsService;
		}

		#region Get
		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> Subcontractors([FromQuery] string? key = null, [FromQuery] Guid? projectId = null, [FromQuery] bool excludeExisting = false)
		{
			List<SubContractorDto> subcontractors = new List<SubContractorDto>();

			if (projectId == null || projectId == Guid.Empty)
			{
				subcontractors = await subcontractorsService.GetAllSubcontractorsAsync(key);
			}
			else
			{
				subcontractors = await subcontractorsService.GetSubContractorsByProjectIdAsync(projectId.Value, excludeExisting);
			}

			return Ok(ApiResponse<List<SubContractorDto>>.SuccessResponse(subcontractors));
		}

		[HttpGet("{id:guid}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> GetSubcontractorById(Guid id)
        {
			try
			{
				var subcontractor = await subcontractorsService.GetSubcontractorByIdAsync(id);
				if (subcontractor == null)
				{
					return NotFound(ApiResponse<string>.ErrorResponse("Subcontractor not found."));
				}
				return Ok(subcontractor);
			}
			catch (Exception ex)
			{
				return NotFound(ApiResponse<string>.ErrorResponse(ex.Message));
			}
        }

        [HttpGet("{id:guid}/projects")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSubContractorWithProjectsAsync(Guid id)
        {
            try
            {
                var subcontractor = await subcontractorsService.GetSubContractorWithProjectsAsync(id);
                if (subcontractor == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse("Subcontractor not found."));
                }
                return Ok(ApiResponse<SubContractorProjectDto>.SuccessResponse(subcontractor));
            }
            catch (Exception ex)
            {
                return NotFound(ApiResponse<SubContractorProjectDto>.ErrorResponse(ex.Message));
            }
        }

        [HttpGet("types")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetSubcontractorTypes()
		{
			var result = await subcontractorsService.GetSubcontractorTypesAsync();
			return Ok(ApiResponse<List<string>>.SuccessResponse(result));
		}


        [HttpPatch("delete/{id}")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status404NotFound)]
		public async Task<IActionResult> DeleteSubcontractor([FromRoute] Guid id)
        {
            var result = await subcontractorsService.DeleteAsync(id);
            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSubcontractor([FromBody] SubContractorDto subContractorDto)
        {
            if (subContractorDto == null)
            {
                return BadRequest(ApiResponse<SubContractorDto>.ErrorResponse("Subcontractor data is null."));
            }

            var result = await subcontractorsService.CreateSubcontractorAsync(subContractorDto);

			return Ok(ApiResponse<SubContractorDto>.SuccessResponse(result));
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut]
        public async Task<IActionResult> UpdateSubContractorAsync([FromBody] SubContractorDto subContractorDto)
        {
            if (subContractorDto == null)
            {
                return BadRequest(ApiResponse<SubContractorDto>.ErrorResponse("SubContractor data is null."));
            }

            await subcontractorsService.UpdateSubcontractorAsync(subContractorDto);
            return Ok(ApiResponse<string>.SuccessResponse("Updated successfully."));
        }


        #endregion

        [HttpDelete("project/{projectId}/delete/{id}/")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProjectSubcontractor([FromRoute] Guid id, Guid projectId)
        {
            var result = await subcontractorsService.DeleteProjectSubContractorAsync(id, projectId);
            return Ok(ApiResponse<bool>.SuccessResponse(result));
        }
    }
}
