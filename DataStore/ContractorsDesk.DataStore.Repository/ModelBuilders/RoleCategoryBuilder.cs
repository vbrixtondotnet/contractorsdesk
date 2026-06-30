using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class RoleCategoryBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<RoleCategory>()
            .HasMany(r => r.Roles)
            .WithOne(r => r.Category)
            .HasForeignKey(rc => rc.CategoryId)
            .IsRequired();

            builder.Entity<RoleCategory>().HasData(
                new RoleCategory { Id = 1, Name = "System" },
                new RoleCategory { Id = 2, Name = "Company" }
            );
        }
    }
    
}
