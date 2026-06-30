using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IQbAccountService : IBaseService
	{
		Task<List<QBAccountDto>> GetQbAccountsAsync();
	}
}
