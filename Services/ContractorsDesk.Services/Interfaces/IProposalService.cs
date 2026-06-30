using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IProposalService : IBaseService
	{
		Task<ProposalDto> NewProposalAsync();
		Task<ProposalDto> GetProposalAsync(Guid id, bool includeZeroAmounts = true);
		Task<ProposalDto> GetProposalDetailsAsync(Guid id);
		Task<ProposalDto> SaveProposalAsync(ProposalModel model, bool merge = false, bool overwrite = false);
		Task<List<ProposalManagementDto>> GetProposalsAsync(int? supervisorId, bool isArchived = false);
		Task<List<ProposalLineDto>> GetProposalLinesAsync(Guid proposalId);
		Task<Proposal> UpdateStatusAsync(UpdateProposalStatusPayload payload);
		Task<List<ProposalTemplateLineItemDto>> GetLineItemsByNameAndTemplateAsync(Guid proposalTemplateId, string name);
		Task<bool> CheckActiveProjectAsync(string projectName);
		Task<bool> IsLineItemAdded(Guid proposalId, string name);
		Task<bool> DeleteProposal(Guid id);
		Task<bool> ArchiveProposal(Guid id, bool isArchived = true);
		Task<List<ProposalCsvDto>> GetProposalLinesForExportGroupedAsync(Guid id);
		Task UpdateProposalIncludeZeroAmount(Guid id);
		Task<bool> CheckIfProposalIncludesZeroAmount(Guid id);
        Task<List<ProjectMatchResultDto>?> GetMatchingProjectsByNameAsync(string projectName, Guid? id = null);
		Task<List<ProjectMatchResultDto>?> GetMatchingDraftProposalsByNameAsync(string projectName, Guid? id = null);
		Task<bool> ValidateClientEmailAddress(ClientModel model);

	}
}
