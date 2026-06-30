using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;
public interface IPermissionsService : IBaseService
{
	Task<List<PermissionDto>> GetByCategoryAsync(int categoryId);
    Task<List<PermissionDto>> GetByRoleIdAsync(int roleId);
	Task<List<ApplicationUserDto>> GetUsersByPermission(int id);
}