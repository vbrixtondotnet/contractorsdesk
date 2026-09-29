using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IEstimateDataMappingService : IBaseService
	{
		Task<List<EstimateCategoryShortDetailsDto>> GetEstimateCategoriesAsync();
		Task<List<EstimateCategoryShortDetailsDto>> GetParentEstimateCategoriesAsync();
		Task<EstimateDataMappingPageDto> GetEstimateDataMappingsAsync();
		Task<EstimateDataMappingPageDto> SaveEstimateDataMappingsAsync(List<EstimateAccountMappingDto> estimateDataMappings);
		Task<List<EstimateCategoryShortDetailsDto>> SaveEstimateCategoriesAsync(List<EstimateCategoryShortDetailsDto> estimateCategories);
	}
}
