using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IContractService
	{
		Task<List<ContractDto>> GetContracts();
		Task<ContractDto> SaveContract(ContractPayload model);
	}
}
