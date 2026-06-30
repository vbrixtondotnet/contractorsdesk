using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IScheduleService : IBaseService
	{
		Task<ProjectScheduleViewDto> GetProjectScheduleAsync(Guid projectId);
		Task<ScheduleRevisionDto> GetScheduleRevision(Guid scheduleRevisionId);
		Task<ProjectScheduleViewDto> SaveProjectScheduleAsync(Guid projectId, ProjectSchedulePayload payload);
		Task<ProjectScheduleDelayDto> AddProjectDelay(Guid projectId, ProjectScheduleDelayDto projectScheduleDelayDto);
		Task<List<ProposalLineItemDto>> GetUnmappedProposalLines(Guid projectId);
		Task<ProjectScheduleDto> GetProjectScheduleConstructionTasks(Guid projectId);
	}
}
