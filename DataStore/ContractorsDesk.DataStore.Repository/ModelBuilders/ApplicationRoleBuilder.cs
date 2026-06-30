using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class ApplicationRoleBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = 1, CategoryId = 1, Name = "Super Admin", NormalizedName = "Super Admin" },
                new ApplicationRole { Id = 2, CategoryId = 1, Name = "Customer Support", NormalizedName = "Customer Support" },
                new ApplicationRole { Id = 3, CategoryId = 1, Name = "Super IT", NormalizedName = "Super IT" },
                new ApplicationRole { Id = 4, CategoryId = 2, Name = "Company Owner", NormalizedName = "Company Owner" },
                new ApplicationRole { Id = 5, CategoryId = 2, Name = "Project Manager", NormalizedName = "Project Manager" },
				new ApplicationRole { Id = 6, CategoryId = 2, Name = "Assistant Project Manager", NormalizedName = "Assistant Project Manager" },
				new ApplicationRole { Id = 7, CategoryId = 2, Name = "Bookkeeper", NormalizedName = "Bookkeeper" },
                new ApplicationRole { Id = 8, CategoryId = 2, Name = "Company IT", NormalizedName = "Company IT" },
                new ApplicationRole { Id = 9, CategoryId = 2, Name = "Office Manager", NormalizedName = "Office Manager" },
				new ApplicationRole { Id = 10, CategoryId = 2, Name = "Client", NormalizedName = "Client" }

			);
        }
    }
    
}
