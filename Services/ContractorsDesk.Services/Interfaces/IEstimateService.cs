using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IEstimateService : IBaseService
	{
		Task<List<EstimateCategoryDto>> GetAllEstimateCategoriesAsync(bool includeParents = true);
		Task<EstimateToActualDto> GetEstimateToActualAsync(Guid proposalId);
		Task<List<EstimateCategoryDto>> GetEstimateItemsByNameAsync(string name);
		Task<List<EstimateCategoryDto>> GetEstimateCategoriesByNameAsync(string name);
		//Task<> GetEstimateCategoriesByNameAsync(string name);
		Task<CostRevisionDto?> SaveRevisedEstimateAsync(Guid proposalId, List<RevisedEstimateCategoryDto> categories);
		Task<bool> SaveRevisedEstimateMappingAsync(RevisedEstimateMappingPayload payload);
		Task<bool> SaveEstimateCategoryAsync(EstimateCategoryPayload payload);
		Task<List<EstimateCategoryShortDetailsDto>> GetEstimateCategoriesByProjectId(Guid id);
		Task<int> UpdateMinimumRequestedAmount(Guid projectId, decimal amount);

	}
}
