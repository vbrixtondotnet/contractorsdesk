using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class ApplicationRoleClaimBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<ApplicationRoleClaim>()
            .HasOne(rc => rc.Permission)
            .WithMany(p => p.RoleClaims)
            .HasForeignKey(rc => rc.PermissionId)
            .IsRequired();

            builder.Entity<ApplicationRoleClaim>()
            .HasOne(rc => rc.Role)
            .WithMany(r => r.Claims)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();

			builder.Entity<ApplicationRoleClaim>().HasData(
			   new ApplicationRoleClaim { Id = 1, PermissionId = 1, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 2, PermissionId = 3, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 3, PermissionId = 5, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 4, PermissionId = 7, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 5, PermissionId = 8, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 6, PermissionId = 9, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 7, PermissionId = 10, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 8, PermissionId = 26, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 9, PermissionId = 27, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 10, PermissionId = 28, RoleId = 4 },
			   new ApplicationRoleClaim { Id = 11, PermissionId = 1, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 12, PermissionId = 3, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 13, PermissionId = 5, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 14, PermissionId = 9, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 15, PermissionId = 10, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 16, PermissionId = 26, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 17, PermissionId = 27, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 18, PermissionId = 28, RoleId = 5 },
			   new ApplicationRoleClaim { Id = 19, PermissionId = 2, RoleId = 6 },
			   new ApplicationRoleClaim { Id = 20, PermissionId = 4, RoleId = 6 },
			   new ApplicationRoleClaim { Id = 21, PermissionId = 6, RoleId = 6 },
			   new ApplicationRoleClaim { Id = 22, PermissionId = 9, RoleId = 6 },
			   new ApplicationRoleClaim { Id = 23, PermissionId = 10, RoleId = 7 },
			   new ApplicationRoleClaim { Id = 24, PermissionId = 14, RoleId = 7 },
			   new ApplicationRoleClaim { Id = 25, PermissionId = 15, RoleId = 7 },
			   new ApplicationRoleClaim { Id = 26, PermissionId = 11, RoleId = 8 },
			   new ApplicationRoleClaim { Id = 27, PermissionId = 12, RoleId = 8 },
			   new ApplicationRoleClaim { Id = 28, PermissionId = 1, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 29, PermissionId = 3, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 30, PermissionId = 5, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 31, PermissionId = 7, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 32, PermissionId = 9, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 33, PermissionId = 26, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 34, PermissionId = 27, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 35, PermissionId = 28, RoleId = 9 },
			   new ApplicationRoleClaim { Id = 36, PermissionId = 29, RoleId = 10 },
			   new ApplicationRoleClaim { Id = 37, PermissionId = 30, RoleId = 8 }

		   );

		}
    }
    
}
