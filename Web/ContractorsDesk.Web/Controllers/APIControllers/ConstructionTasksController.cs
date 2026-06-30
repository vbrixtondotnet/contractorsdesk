using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[Route("api/construction-tasks")]
	[ApiController]
	public class ConstructionTasksController : BaseApiController
	{
		private readonly IConstructionTasksService constructionTasksService;
        private readonly IMapper mapper;
		public ConstructionTasksController(IConstructionTasksService constructionTasksService, 
			IPermissionsService permissionsService,
			IApplicationUserService applicationUserService,
			IMapper mapper, 
			IHttpContextAccessor httpContextAccessor) : 
			base(httpContextAccessor, permissionsService, applicationUserService) 
		{
			this.mapper = mapper;	
			this.constructionTasksService = constructionTasksService;
			this.constructionTasksService.UserId = this.UserId;
		}

		#region GET

		[HttpGet()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> GetAllConstructionTasks()
		{
			var constructionTasks = await this.constructionTasksService.GetAllConstructionTaskAsync();
			return Ok(ApiResponse<List<ConstructionTaskDto>>.SuccessResponse(constructionTasks));

		}
		#endregion

		#region POST
		[HttpPost()]
		[ProducesResponseType(StatusCodes.Status200OK)]
		public async Task<IActionResult> SaveNewConstructionTask([FromBody] ConstructionTaskPayload payload)
		{
			var constructionTasks = await this.constructionTasksService.CreateConstructionTaskAsync(payload);
			return Ok(ApiResponse<ConstructionTaskDto>.SuccessResponse(constructionTasks));

		}
		#endregion

		#region PUT

		#endregion

	}
}
