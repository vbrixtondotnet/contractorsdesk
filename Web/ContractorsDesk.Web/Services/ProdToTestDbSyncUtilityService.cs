using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.WebPortal.Helpers;
using Hangfire;
namespace ContractorsDesk.WebPortal.Services
{
	[DisableRetries]
	public class ProdToTestDbSyncUtilityService
	{
		private readonly IImportDataService prodToTestDbSyncService;

		public ProdToTestDbSyncUtilityService(IImportDataService prodToTestDbSyncService)
		{
			this.prodToTestDbSyncService = prodToTestDbSyncService;
		}

		[DisableRetries]
		public async Task ExecuteSync()
		{
			await prodToTestDbSyncService.ImportDataFromProductionDatabase();
		}
	}
}
