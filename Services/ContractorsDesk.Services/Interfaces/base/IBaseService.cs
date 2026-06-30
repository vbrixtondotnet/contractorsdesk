using ContractorsDesk.DataStore.Client.Models;

namespace ContractorsDesk.Services.Interfaces.@base
{
	public interface IBaseService
	{
		Task<T> CreateAsync<T>(object param);
        Task<T> UpdateAsync<T>(object param);
        Task DeleteAsync(object param);
        Task<T> GetAllAsync<T>();
		int UserId { get; set; }
		ClientDbContext? ClientDbContext { get; set; }
	}

}
