using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class PermissionBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<Permission>()
             .HasOne(rc => rc.Category)
             .WithMany(r => r.Permissions)
             .HasForeignKey(rc => rc.CategoryId)
             .IsRequired();

            var dateCreated = DateTime.Parse("10/5/2024 20:30");
			builder.Entity<Permission>().HasData(
				new Permission { Id = 1, CategoryId = 2, Description = "Manage All Jobs", DateCreated = dateCreated },
				new Permission { Id = 2, CategoryId = 2, Description = "Manage Own Jobs", DateCreated = dateCreated },
				new Permission { Id = 3, CategoryId = 2, Description = "Manage All Estimates", DateCreated = dateCreated },
				new Permission { Id = 4, CategoryId = 2, Description = "Manage Own Estimates", DateCreated = dateCreated },
				new Permission { Id = 5, CategoryId = 2, Description = "Manage All Action Items", DateCreated = dateCreated },
				new Permission { Id = 6, CategoryId = 2, Description = "Manage Own Action Items", DateCreated = dateCreated },
				new Permission { Id = 7, CategoryId = 2, Description = "Manage Company Users", DateCreated = dateCreated },
				new Permission { Id = 8, CategoryId = 2, Description = "Manage Company Roles", DateCreated = dateCreated },
				new Permission { Id = 9, CategoryId = 2, Description = "Manage Schedules", DateCreated = dateCreated },
				new Permission { Id = 10, CategoryId = 2, Description = "Manage Finance", DateCreated = dateCreated },
				new Permission { Id = 11, CategoryId = 2, Description = "Manage Client Websites", DateCreated = dateCreated },
				new Permission { Id = 12, CategoryId = 2, Description = "Manage Company Email", DateCreated = dateCreated },
				new Permission { Id = 13, CategoryId = 2, Description = "Manage Company SEO", DateCreated = dateCreated },
				new Permission { Id = 14, CategoryId = 2, Description = "Manage Client Onboarding", DateCreated = dateCreated },
				new Permission { Id = 15, CategoryId = 2, Description = "Access to Third-party Services", DateCreated = dateCreated },
				new Permission { Id = 16, CategoryId = 1, Description = "Manage System Users", DateCreated = dateCreated },
				new Permission { Id = 17, CategoryId = 1, Description = "Manage System Roles", DateCreated = dateCreated },
				new Permission { Id = 18, CategoryId = 1, Description = "Manage Account Owners", DateCreated = dateCreated },
				new Permission { Id = 19, CategoryId = 1, Description = "Manage System Permissions", DateCreated = dateCreated },
				new Permission { Id = 20, CategoryId = 1, Description = "Manage Client Websites", DateCreated = dateCreated },
				new Permission { Id = 21, CategoryId = 1, Description = "Manage Client Onboarding", DateCreated = dateCreated },
				new Permission { Id = 22, CategoryId = 1, Description = "Access to Third-party Services", DateCreated = dateCreated },
				new Permission { Id = 23, CategoryId = 1, Description = "Manage Company Email", DateCreated = dateCreated },
				new Permission { Id = 24, CategoryId = 1, Description = "Manage Company SEO", DateCreated = dateCreated },
				new Permission { Id = 25, CategoryId = 1, Description = "Manage Clients", DateCreated = dateCreated },
				new Permission { Id = 26, CategoryId = 2, Description = "Can Assign Jobs", DateCreated = dateCreated },
				new Permission { Id = 27, CategoryId = 2, Description = "Can Assign Estimates", DateCreated = dateCreated },
				new Permission { Id = 28, CategoryId = 2, Description = "Can Assign Action Items", DateCreated = dateCreated },
				new Permission { Id = 29, CategoryId = 2, Description = "Can Access Client Jobs", DateCreated = dateCreated },
				new Permission { Id = 30, CategoryId = 2, Description = "Can Manage Data Mapping", DateCreated = dateCreated },
                new Permission { Id = 31, CategoryId = 2, Description = "Can Edit Accepted Proposals", DateCreated = dateCreated },
                new Permission { Id = 32, CategoryId = 2, Description = "Can Edit Completed Schedules", DateCreated = dateCreated }
            );
        }
    }
    
}
