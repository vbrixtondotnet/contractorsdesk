using ContractorsDesk.Core.Dto;

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.Services.@base;
using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services
{
	public class PermissionsService : BaseService, IPermissionsService
	{
		public PermissionsService(ClientDbContext clientDbContext, IMapper mapper) : base(mapper, clientDbContext) {}

		public override async Task<T> GetAllAsync<T>()
		{
			var permissions = new List<PermissionDto>();
			var dbPermissions = await ClientDbContext.Permissions
				.Include(p => p.RolePermissions)
					.ThenInclude(p=> p.Permission)
				.ToListAsync();

			return mapper.Map<T>(dbPermissions);
		}
		public async Task<List<PermissionDto>> GetByCategoryAsync(int categoryId)
		{
			var permissions = await ClientDbContext.Permissions.Where(p=> p.PermissionType == categoryId).ToListAsync();
			return mapper.Map<List<PermissionDto>>(permissions);
		}
		public Task<List<PermissionDto>> GetByRoleIdAsync(int roleId)
		{
            return Task.Run(() =>
			{
                var permissions = ClientDbContext.Roles.Where(r => r.Id == roleId)
                            .Include(r => r.RolePermissions)
                            .ThenInclude(c => c.Permission)
                            .SelectMany(r => r.RolePermissions.Select(c => c.Permission))
                            .ToList();

                return mapper.Map<List<PermissionDto>>(permissions);
            });
        }

		public async Task<List<ApplicationUserDto>> GetUsersByPermission(int id)
		{
			throw new NotImplementedException();
			//var dbUsersWithPermission = await clientDbContext.Users
			//.Where(user => clientDbContext.Roles
			//	.Any(ur => ur.Id == user.Id && clientDbContext.RolePermissions
			//		.Any(rc => rc.RoleId == ur.Id && rc.PermissionId == id)))
			//.ToListAsync();


			//var dbUserRoles = await clientDbContext.Roles
			//	.Include(ur => ur.RolePermissions)
			//		.ThenInclude(Permission => Permission.Permission)
			//	.Where(ur => dbUsersWithPermission.Select(user => user.RoleId).Contains(ur.Id))
			//	.ToListAsync();

			//var dbRoleClaims = await clientDbContext.RolePermissions
			//	.Include(rc => rc.Permission)
			//	.Where(rc => dbUserRoles.Select(ur => ur.Id).Contains(rc.RoleId) && rc.PermissionId == id)
			//	.ToListAsync();


			//var userDtos = dbUsersWithPermission.Select(user => new ApplicationUserDto
			//{
			//	Id = user.Id,
			//	FirstName = user.FirstName,
			//	LastName = user.LastName,
			//	Email = user.Email,
			//	EmailConfirmed = user.EmailConfirmed,
			//	RequireLogOn = user.RequireLogOn,
			//	CreatedDate = user.CreatedDate,
			//	Roles = dbUserRoles
			//		.Where(ur => ur.UserId == user.Id)
			//		.Select(ur =>
			//		{
			//			var roleClaims = dbRoleClaims.Where(rc => rc.RoleId == ur.RoleId).ToList();
			//			return new RoleDto
			//			{
			//				Id = ur.RoleId,
			//				Name = ur.Role?.Name ?? "",
			//				Permissions = roleClaims
			//					.Where(rc => rc.Permission != null)
			//					.Select(rc => new PermissionDto
			//					{
			//						Id = rc.Permission.Id,
			//						Description = rc.Permission.Description
			//					})
			//					.ToList()
			//			};
			//		})
			//		.ToList()
			//}).ToList();


			//return this.mapper.Map<List<ApplicationUserDto>>(userDtos);

		}
	}
}
