using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IUserLogService : IBaseService
	{
		Task CreateLogAsync(string path, int userId);
    }
}
