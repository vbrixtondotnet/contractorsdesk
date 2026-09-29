using AutoMapper;
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
	[Route("api")]
	[ApiController]
	public class EstimateDataMappingController : BaseApiController
	{
		private readonly IEstimateDataMappingService estimateDataMappingService;
		public EstimateDataMappingController(
			IEstimateDataMappingService estimateDataMappingService,
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.estimateDataMappingService = estimateDataMappingService;

			this.estimateDataMappingService.UserId = this.UserId;
		}

		#region GET
		[HttpGet("estimate-data-mappings")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEstimateDataMappings()
		{
			var response = await estimateDataMappingService.GetEstimateDataMappingsAsync();
			return Ok(ApiResponse<EstimateDataMappingPageDto>.SuccessResponse(response));
		}

		[HttpGet("estimate-data-mappings/estimate-categories")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetEstimateCategories()
		{
			var response = await estimateDataMappingService.GetEstimateCategoriesAsync();
			return Ok(ApiResponse<List<EstimateCategoryShortDetailsDto>>.SuccessResponse(response));
		}

		[HttpGet("estimate-data-mappings/parent-estimate-categories")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetParentEstimateCategories()
		{
			var response = await estimateDataMappingService.GetParentEstimateCategoriesAsync();
			return Ok(ApiResponse<List<EstimateCategoryShortDetailsDto>>.SuccessResponse(response));
		}
		#endregion

		#region POST

		[HttpPost("estimate-data-mappings")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveEstimateDataMappings([FromBody] List<EstimateAccountMappingDto> mappings)
		{
			var response = await estimateDataMappingService.SaveEstimateDataMappingsAsync(mappings);
			return Ok(ApiResponse<EstimateDataMappingPageDto>.SuccessResponse(response));
		}


		[HttpPost("estimate-data-mappings/parent-estimate-categories")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveEstimateCategories([FromBody] List<EstimateCategoryShortDetailsDto> estimateCategories)
		{
			var response = await estimateDataMappingService.SaveEstimateCategoriesAsync(estimateCategories);
			return Ok(ApiResponse<List<EstimateCategoryShortDetailsDto>>.SuccessResponse(response));
		}

		#endregion

		//[HttpPost("revised-estimates/{id}")]
		//[ProducesResponseType(StatusCodes.Status200OK)]
		//public async Task<IActionResult> SaveRevisedEstimates(Guid id, [FromBody] List<RevisedEstimateCategoryDto> categories)
		//{
		//	var response = await estimateService.SaveRevisedEstimateAsync(id, categories);
		//	return Ok(ApiResponse<bool>.SuccessResponse(response));
		//}

	}
}
