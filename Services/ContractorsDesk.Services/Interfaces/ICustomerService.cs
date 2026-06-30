using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface ICustomerService : IBaseService
	{
		Task<List<ClientDto>> SearchCustomersAsync(string searchKey);
    }
}
