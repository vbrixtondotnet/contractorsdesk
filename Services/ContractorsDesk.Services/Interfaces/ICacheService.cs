using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface ICacheService : IBaseService
	{
		Task<T?> GetFromCache<T>(string key);
		Task SetAsync<T>(string key, T value);
		Task RemoveAsync(string key);
	}
}
