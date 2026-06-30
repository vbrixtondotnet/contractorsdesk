using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services
{
	public class RolesService : BaseService, IRolesService
	{
		public RolesService(ClientDbContext clientDbContext, IMapper mapper) 
			: base(mapper, clientDbContext) { }

		public override async Task<T> GetAllAsync<T>()
		{
			List<RoleDto> roles = new List<RoleDto>();
			var dbRoles = await this.ClientDbContext.Roles
				.Include(r => r.RolePermissions)
				.ThenInclude(c => c.Permission)
				.ToListAsync();

			foreach (var role in dbRoles)
			{
				var permissions = role.RolePermissions.Select(c => c.Permission).ToList();
			}

			return mapper.Map<T>(dbRoles); ;
		}

		public async Task<RoleDto> GetByIdAsync(int roleId)
		{
			var dbRole = await ClientDbContext.Roles
				.Include(r => r.RolePermissions)
				.ThenInclude(c => c.Permission)
				.FirstOrDefaultAsync(r => r.Id == roleId);

			return mapper.Map<RoleDto>( dbRole );
		}

		public async Task<List<ApplicationUserDto>> GetRoleUsersAsync(int roleId, int? companyId)
		{
			List<ApplicationUserDto> roles = new List<ApplicationUserDto>();

			var dbUsers = await ClientDbContext.Users
				.Include(x => x.Role)
				.Where(x => x.Role.Id == roleId)
				.ToListAsync();

			return mapper.Map<List<ApplicationUserDto>>(dbUsers);

		}

		public async Task<List<RoleDto>> GetRolesByCategoryAsync(int roleCategoryId)
		{
			var dbRoles = await ClientDbContext.Roles
				.Include(r => r.RolePermissions)
				.ThenInclude(c => c.Permission)
				.Where(r => r.RoleType == roleCategoryId)
				.ToListAsync();

			return mapper.Map<List<RoleDto>>(dbRoles);
		}

		public async Task<List<ApplicationUserDto>> GetSupervisorsAsync()
		{
			List<int> supervisorRoleIds = [(int)Roles.AssistantProjectManager, (int)Roles.ProjectManager];

			var users = await ClientDbContext.Users
				.Where(ur => supervisorRoleIds.Contains(ur.RoleId))
				.ToListAsync();

			return mapper.Map<List<ApplicationUserDto>>(users);
		}

		public override async Task<T> UpdateAsync<T>(object param)
		{
			if (param is Core.ApiPayloadModels.Role role)
			{
				var dbRole = await ClientDbContext.Roles
					.Include(r=> r.RolePermissions)
					.ThenInclude(r=> r.Permission)
					.FirstOrDefaultAsync(r=> r.Id == role.Id);

				if (dbRole == null) throw new Exception("Role does not exist.");

				dbRole.Name = role.Name;

				var rolePermissions = await ClientDbContext.RolePermissions.Where(rc => rc.RoleId == role.Id).ToListAsync();
                ClientDbContext.RemoveRange(rolePermissions);

				foreach (var permission in role.Permissions)
				{
                    ClientDbContext.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
				}
				await ClientDbContext.SaveChangesAsync();
				return mapper.Map<T>(dbRole);
			}
			else
			{
				throw new ArgumentException("Invalid payload type for UpdateAsync");
			}
		}
	}
}
