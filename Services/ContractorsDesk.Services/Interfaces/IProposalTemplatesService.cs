using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IProposalTemplatesService : IBaseService
	{
		Task<ProposalTemplateDto> GetByIdAsync(Guid id);
		Task<List<ProposalTemplateLineItemDto>> GetLineItemsAsync(Guid id);
		Task<List<ProposalTemplateDto>> GetProposalTemplatesAsync();
		Task<ProposalTemplateDto> CreateProposalTemplateAsync(ProposalTemplatePayload payload);
		Task<ProposalTemplateDto> UpdateProposalTemplateAsync(ProposalTemplatePayload payload);
		Task<ProposalTemplateUserDefaultDto?> GetProposalTemplateUserDefault();
		Task SaveProposalTemplateUserDefault(Guid id);
		Task<bool> RemoveProposalTemplate(Guid id);
        Task<bool> DeleteProposalTemplate(Guid id);
    }
}
