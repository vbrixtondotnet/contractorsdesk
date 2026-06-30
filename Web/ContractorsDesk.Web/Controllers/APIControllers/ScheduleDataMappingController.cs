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
	public class ScheduleDataMappingController : BaseApiController
	{
		private readonly IScheduleteDataMappingService scheduleDataMappingService;
		public ScheduleDataMappingController(IScheduleteDataMappingService scheduleDataMappingService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.scheduleDataMappingService = scheduleDataMappingService;
			this.scheduleDataMappingService.UserId = this.UserId;
		}

		[HttpGet("schedule-data-mappings")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetScheduleDataMappingAsync()
		{
			var response = await scheduleDataMappingService.GetScheduleDataMappingsAsync();
			return Ok(ApiResponse<List<ScheduleDataWithTaskMappingDto>>.SuccessResponse(response));
		}

		[HttpPost("schedule-data-mappings")]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveEstimateDataMappings([FromBody] List<TaskMappingDto> mappings)
		{
			var response = await scheduleDataMappingService.SaveScheduleteDataMappingsAsync(mappings);
			return Ok(ApiResponse<List<ScheduleDataWithTaskMappingDto>>.SuccessResponse(response));
		}
	}
}
