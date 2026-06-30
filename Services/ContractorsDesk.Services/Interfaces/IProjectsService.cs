using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IProjectsService : IBaseService
	{
		Task<List<ProjectDto>> GetProjectsBySupervisorIdAsync();
		Task<List<ProjectDetailsDto>> GetActiveJobsByUserAsync(Roles role, int userId, bool canManageAllProjects, string? searchKey = null);
		Task<List<ProjectDetailsDto>> GetArchivedJobsByUserAsync(Roles role, int userId, bool canManageAllProjects);
		Task<ProjectDetailsDto> GetProjectShortDetailsAsync(Guid id);
		Task<List<ProjectDto>> GetActiveProjectsAsync();
		Task<List<ProjectDto>> GetPendingProjectsAsync();
		Task<ProjectDto> GetProjectByIdAsync(Guid id);
		Task<ProjectDetailsViewDto> GetProjectDetailsByIdAsync(Guid id);
		Task<List<ClientProjectDto>> GetClientProjects();
		Task<ClientDto> UpdateClientDetailsAsync(Guid projectId, ClientModel clientModel);
		Task UpdateProjectPhotoUrl(Guid projectId, string photoUrl);
		Task<ProposalDto> GetProjectProposal(Guid projectId);
		Task<ProjectDto> GetProjectByProposalIdAsync(Guid proposalId);
		Task<ProjectDocumentDto> AddProjectDocumentAsync(ProjectDocumentDto projectDocumentDto);
		Task<List<ProjectSupervisorDto>> GetProjectSupervisorsAsync(Guid projectId);
		Task<ProjectEstimateCategoriesAndScheduleItemsDto> GetProjectEstimateCategoriesAndScheduleItemsAsync(Guid projectId);
		Task<SubContractorDto> AddSubcontractorAsync(SubContractorPayloadModel model);
		Task<List<ActiveProjectDto>> GetSupervisorActiveJobs(Roles role, int supervisorId, bool canManageAllProjects);
		Task ArchiveProjectAsync(Guid id);
        Task<ProjectNoteDto> SaveProjectNote(ProjectNoteModel model);
		Task DeleteProjectAsync(Guid id);
		Task UnArchiveProjectAsync(Guid id);

    }
}
