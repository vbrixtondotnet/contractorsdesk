using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IActivityStreamService : IBaseService
	{
		Task CreateActivityStream(ActivityStreamPayload activityStreamPayload);
	}
}
