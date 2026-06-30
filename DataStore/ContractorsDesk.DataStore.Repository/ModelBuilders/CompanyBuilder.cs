using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders
{
    public class CompanyBuilder : IModelBuilder
    {
        public void Build(ModelBuilder builder)
        {
            builder.Entity<Company>().HasData(
                           new Company { Id = 1, Name = "CH Anderson Construction" }
                       );
        }
    }
    
}
