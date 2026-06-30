using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IEstimateDataMappingService : IBaseService
	{
		Task<List<EstimateCategoryShortDetailsDto>> GetEstimateCategoriesAsync();
		Task<List<EstimateCategoryShortDetailsDto>> GetParentEstimateCategoriesAsync();
		Task<List<EstimateDataMappingDto>> GetEstimateDataMappingsAsync();
		Task<List<EstimateDataMappingDto>> SaveEstimateDataMappingsAsync(List<EstimateDataMappingDto> estimateDataMappings);
		Task<List<EstimateCategoryShortDetailsDto>> SaveEstimateCategoriesAsync(List<EstimateCategoryShortDetailsDto> estimateCategories);
	}
}
