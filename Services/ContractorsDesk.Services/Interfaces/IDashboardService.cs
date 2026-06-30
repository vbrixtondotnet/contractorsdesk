using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IDashboardService : IBaseService
	{
		Task<DashboardStatsDto> GetDashboardStatsAsync(bool canManageAllProjects); 
	}
}
