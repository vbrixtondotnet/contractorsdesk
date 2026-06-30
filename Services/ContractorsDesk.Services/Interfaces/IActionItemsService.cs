using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IActionItemsService : IBaseService
	{
		Task<ActionItemDto> GetActionItemAsync(int id);
		Task<ActionItemShortDetailsDto> GetActionItemShortDetailsAsync(int id, bool includeComments = true);
		Task<List<ActionTypeDto>> GetActionTypesAsync();
		Task<List<ActionItemDto>> GetActionItemsAsync(int? supervisorId);
		Task<List<ActionItemDto>> GetActionItemsByStatusAsync(int? supervisorId, int statusId);
		Task<ActionItemSummaryViewDto> GetActionItemSummaryViewByIdAsync(int id);
		Task<List<ActionItemSummaryViewDto>> GetDashboardActionItemsAsync(bool loadAll, int supervisorId, int? statusId, Guid? projectId = null);
		Task<List<ActionItemDto>> GetAllActionItemsCreatedInLastDaysAsync(int days);
		Task<List<ActionItemDto>> GetActionItemsCreatedSinceAsync(DateTime? dateFrom = null, Guid? projectId = null);
		Task<List<ActionItemSummaryViewDto>> GetByProjectIdAsync(Guid projectId, int? userId = null);
		Task<List<ActionItemDto>> CreateActionItems(List<ActionItemPayload> payload);
		Task<ActionItemSummaryViewDto> CreateNote(ActionItemNotePayload payload);
		Task<ActionItemSummaryViewDto> AcceptActionItemAsync(int id);
		Task<ActionItemSummaryViewDto> CompleteActionItemAsync(int id);
		Task<ActionItemSummaryViewDto> ApproveActionItemAsync(ActionItemSummaryViewDto actionItem);
		Task<ActionItemSummaryViewDto> AcknowledgeActionItemAsync(ActionItemSummaryViewDto actionItem);
		Task<ActionItemDto> ArchiveActionItemAsync(int id, bool toArchive = false);
		Task<ActionItemCostChangeDto> GetActionItemCostChangeAsync(int actionItemId);
		Task<bool> DeleteActionItemAsync(int id);
		Task<ActionItemSummaryViewDto> UpdateActionItemAsync(int id, ActionItemPayload payload);
		Task<ActionItemShortDetailsDto> UpdateActionItemDetails(ActionItemDetailsUpdatePayload payload);
		Task<ActionItemCommentDto> AddCommentAsync(ActionItemCommentPayload payload);
		Task<bool> UpdateActionItemSupervisors(int id, ActionItemPayload payload);
		Task SetActionItemStatusAsync(int actionItemId, ActionItemStatus status);


	}
}
