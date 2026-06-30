using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IChangeOrderService : IBaseService
	{
		Task<ChangeOrderDto> CreateChangeOrderAsync(ChangeOrderDto changeOrderDto);
		Task<ChangeOrderDto> GetChangeOrderByIdAsync(int id);
		Task<ChangeOrderDto> GetChangeOrderByActionItemIdAsync(int actionItemId);
	}
}
