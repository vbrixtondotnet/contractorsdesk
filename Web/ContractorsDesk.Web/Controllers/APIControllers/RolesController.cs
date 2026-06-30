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
	public class RolesController : BaseApiController
	{
		private readonly IRolesService rolesService;
		public RolesController(IRolesService rolesService, 
			IApplicationUserService applicationUserService,
            IHttpContextAccessor httpContextAccessor,
            IPermissionsService permissionsService) : base(httpContextAccessor, permissionsService, applicationUserService)
		{
			this.rolesService = rolesService;	
		}

		[HttpGet("roles")]
		public async Task<IActionResult> GetRoles()
		{
			var response = await rolesService.GetAllAsync<List<RoleDto>>();
			return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(response));
		}

		[HttpGet("roles/{id}")]
		public async Task<IActionResult> GetRole(int id)
		{
			var role = await rolesService.GetByIdAsync(id);

			if (role == null) return NotFound();

			return Ok(ApiResponse<RoleDto>.SuccessResponse(role));
		}

		[HttpGet("roles/{id}/users")]
		public async Task<IActionResult> GerRoleUsers(int id)
		{
			
			var roleUsers = await rolesService.GetRoleUsersAsync(id, this.CompanyId);

			return Ok(ApiResponse<List<ApplicationUserDto>>.SuccessResponse(roleUsers));
		}

		[HttpGet("roles/supervisors")]
		public async Task<IActionResult> GetSupervisors()
		{
			var roleUsers = await rolesService.GetSupervisorsAsync();

			return Ok(ApiResponse<List<ApplicationUserDto>>.SuccessResponse(roleUsers));
		}

		[HttpPut("roles")]
		[ProducesResponseType(StatusCodes.Status204NoContent)]
		public async Task<IActionResult> GerRoleUsers([FromBody] Core.ApiPayloadModels.Role role)
		{
			var roleDto = await rolesService.UpdateAsync<RoleDto>(role);
			return Ok(ApiResponse<RoleDto>.SuccessResponse(roleDto));
		}

		[HttpGet("role-category/users")]
		public async Task<IActionResult> GetUsersByRoleCategory()
		{
			var usersInRoleCategory = await this.applicationUserService.GetUsersByRoleCategoryAsync(this.RoleCategoryId);

			return Ok(ApiResponse<List<ApplicationUserDto>>.SuccessResponse(usersInRoleCategory));
		}

		[HttpGet("role-category/roles")]
		public async Task<IActionResult> GetRolesByCategory()
		{
			var usersInRoleCategory = await rolesService.GetRolesByCategoryAsync(this.RoleCategoryId);

			return Ok(ApiResponse<List<RoleDto>>.SuccessResponse(usersInRoleCategory));
		}

	}
}
