using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.Core.Models;
using Microsoft.AspNetCore.Identity;
using ContractorsDesk.DataStore.Account.ModelBuilders;

namespace ContractorsDesk.DataStore.Account
{
	public class AccountDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int,
	IdentityUserClaim<int>, ApplicationUserRole, IdentityUserLogin<int>,
	ApplicationRoleClaim, IdentityUserToken<int>>
	{
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<Company> Companies { get; set; }
		public AccountDbContext(DbContextOptions<AccountDbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            new ApplicationUserBuilder().Build(builder);
            new ApplicationRoleClaimBuilder().Build(builder);
            new ApplicationUserRoleBuilder().Build(builder);
            new RoleCategoryBuilder().Build(builder);
            new ApplicationRoleBuilder().Build(builder);
            new CompanyBuilder().Build(builder);
            new PermissionCategoryBuilder().Build(builder);
            new PermissionBuilder().Build(builder);
		}
	}
}
