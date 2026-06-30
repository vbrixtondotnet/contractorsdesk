using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.DataStore.Account.ModelBuilders.Interfaces
{
	public interface IModelBuilder
    {
        void Build(ModelBuilder builder);
    }
}
