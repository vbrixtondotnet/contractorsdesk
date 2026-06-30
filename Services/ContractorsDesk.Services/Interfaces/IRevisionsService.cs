using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IRevisionsService : IBaseService
	{
		Task<CostRevisionDto> CreateCostRevisionAsync(CostRevisionPayload costRevisionPayload);
		Task MarkScheduleRevisionAsSent(Guid scheduleRevisionId);
		Task AssignActionItemToScheduleRevision(Guid scheduleRevisionId, int actionItemId);
		Task AssignActionItemToCostRevision(Guid costRevisionId, int actionItemId);
		Task<ScheduleRevisionDto> CreateScheduleRevisionsAsync(ScheduleRevisionPayload revision, Guid projectId);
		Task<ScheduleRevisionDto> GetScheduleRevisionByIdAsync(Guid id);
		Task<List<ScheduleRevisionDto>> GetScheduleRevisionsByProjectIdAsync(Guid projectId, int? statusId = null);
		Task<CostRevisionDto> GetCostRevisionAsync(Guid id);
	}
}
