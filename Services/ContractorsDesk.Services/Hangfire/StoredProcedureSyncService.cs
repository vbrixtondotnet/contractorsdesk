using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Hangfire.@base;
using Microsoft.Extensions.Configuration;

namespace ContractorsDesk.Services.Hangfire
{
    public class StoredProcedureSyncService : Base
    {
        public StoredProcedureSyncService(IConfiguration configuration)
            : base(configuration)
        { }

        /// <summary>
        /// Executes [dbo].sp_QBClassesActiveJobsSync.
        /// </summary>
        public void RunSP_QBClassesActiveJobsSync()
            => base.ExecuteStoredProcedure("[dbo].sp_QBClassesActiveJobsSync");

    }
}
