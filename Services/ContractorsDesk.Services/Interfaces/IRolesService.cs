using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IRolesService : IBaseService
	{
		Task<RoleDto> GetByIdAsync(int roleId);
		Task<List<ApplicationUserDto>> GetRoleUsersAsync(int roleId, int? companyId);
		Task<List<RoleDto>> GetRolesByCategoryAsync(int roleCategoryId);
		Task<List<ApplicationUserDto>> GetSupervisorsAsync();
	}
}
