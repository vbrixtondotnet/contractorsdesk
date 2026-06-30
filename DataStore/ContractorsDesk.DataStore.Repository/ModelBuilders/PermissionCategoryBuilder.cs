using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class PermissionCategoryBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<PermissionCategory>().HasData(
                new PermissionCategory { Id = 1, Name = "System Permissions" },
                new PermissionCategory { Id = 2, Name = "Company Permissions" }
            );
        }
    }
    
}
