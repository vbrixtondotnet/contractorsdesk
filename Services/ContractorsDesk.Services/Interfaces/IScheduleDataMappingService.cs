using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IScheduleteDataMappingService : IBaseService
	{
		Task<List<ScheduleDataWithTaskMappingDto>> GetScheduleDataMappingsAsync();

		Task<List<ScheduleDataWithTaskMappingDto>> SaveScheduleteDataMappingsAsync(List<TaskMappingDto> scheduleDataMappings);
	}
}
