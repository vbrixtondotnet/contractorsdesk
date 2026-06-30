using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class ApplicationUserRoleBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {

            builder.Entity<ApplicationUserRole>()
            .HasOne(rc => rc.User)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(rc => rc.UserId)
            .IsRequired();

            builder.Entity<ApplicationUserRole>()
            .HasOne(rc => rc.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();
        }
    }
    
}
