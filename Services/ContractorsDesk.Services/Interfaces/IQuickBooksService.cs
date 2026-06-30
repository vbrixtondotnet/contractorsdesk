using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IQuickBooksService
	{
		Task SaveAccessToken(TokenResponsePayload token, string realmId);
		Task RunDataSync(SignalRMessageModel? signalRMessageModel = null);
		Task SyncProjectTotals(SignalRMessageModel? signalRMessageModel = null);
		Task SyncProposals(SignalRMessageModel? signalRMessageModel = null);
		Task SyncActiveJobs(SignalRMessageModel? signalRMessageModel = null);
		Task<bool> HasQuickBooksAccountConnected();
		Task<object> GetProfitAndLossReportAsync(List<long> qbClasslistIds);
		Task SyncQBClassAsync(bool isFirstRun, int daysLookupFilter, SignalRMessageModel signalRMessageModel);
		Task SyncQBCustomerAsync(bool isFirstRun, int daysLookupFilter, SignalRMessageModel signalRMessageModel);
	}
}
