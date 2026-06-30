using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IProjectJournalService : IBaseService
	{
		Task<ProjectJournalDto> SaveAsync(Guid projectId, ProjectJournalPayload projectJournal, bool prepend = false);
		Task<ProjectJournalDto> GetProjectJournalByProjectIdAsync(Guid projectId);
		Task<ProjectJournalDto> GetByIdAsync(Guid id);
		Task<DateTime> GetLatestJournalDateAsync(Guid? projectId);
	}
}
