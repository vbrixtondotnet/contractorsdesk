using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class ApplicationUserBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>()
            .HasOne(rc => rc.Company)
            .WithMany(p => p.Users)
            .HasForeignKey(rc => rc.CompanyId)
            .IsRequired(false);
        }
    }
    
}
