using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IConstructionTasksService : IBaseService
	{
		Task<List<ConstructionTaskDto>> GetAllConstructionTaskAsync();
		Task<ConstructionTaskDto> CreateConstructionTaskAsync(ConstructionTaskPayload payload);
	}
}
