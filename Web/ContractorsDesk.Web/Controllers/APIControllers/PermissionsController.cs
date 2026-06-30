using ContractorsDesk.Core.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace ContractorsDesk.WebPortal.Controllers.APIControllers
{
	[Authorize]
	[Route("api")]
	[Produces(MediaTypeNames.Application.Json)]
	[Consumes(MediaTypeNames.Application.Json)]
	[ApiController]
	public class PermissionsController : ControllerBase
	{
		private readonly IPermissionsService permissionsService;
		public PermissionsController(IPermissionsService permissionsService)
		{
			this.permissionsService = permissionsService;	
		}

		[HttpGet("permissions")]
		public async Task<IActionResult> GetPermissions(int categoryId = 0)
		{
			var permissions = new List<PermissionDto>();
			if(categoryId == 0)
			{
				permissions = await permissionsService.GetAllAsync<List<PermissionDto>>();
			}
			else
			{
				permissions = await permissionsService.GetByCategoryAsync(categoryId);
			}

			return Ok(ApiResponse<List<PermissionDto>>.SuccessResponse(permissions));
		}

		[HttpGet("permissions/{id}/users")]
		public async Task<IActionResult> GetUsersByPermission(int id = 0)
		{
			var users = new List<ApplicationUserDto>();
			if (id == 0)
			{
				return BadRequest("Permission ID is required.");
			}
			else 
			{
				users = await permissionsService.GetUsersByPermission(id);
			}

			return Ok(ApiResponse<List<ApplicationUserDto>>.SuccessResponse(users));
		}
	}
}
