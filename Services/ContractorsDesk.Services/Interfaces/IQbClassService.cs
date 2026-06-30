using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IQbClassService
	{
		Task<List<QbClassDto>> GetActiveQbClassesAsync();
	}
}
