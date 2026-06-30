using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class ScheduleDataMappingService : BaseService, IScheduleteDataMappingService
	{
		public ScheduleDataMappingService(
			ClientDbContext clientDataDbContext, 
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			:base(mapper, clientDataDbContext, masterDbContext, configuration) {}

		public async Task<List<ScheduleDataWithTaskMappingDto>> GetScheduleDataMappingsAsync()
		{

            var parentEstimateCategories = await ClientDbContext.EstimateCategories
                .Where(ec => ec.ParentEstimateCategoryId == null)
                .ToListAsync();

			var scheduleDataMappings = await ClientDbContext.EstimateCategories
				.Where(ec => ec.ParentEstimateCategoryId != null)
				.GroupJoin(
					ClientDbContext.ScheduleTaskMappings.Include(stm => stm.ConstructionTask),
					ec => ec.Id,
					sm => sm.EstimateCategoryId,
					(ec, tasks) => new { EstimateCategory = ec, Tasks = tasks })
				.ToListAsync();

            var result = scheduleDataMappings.Select(data =>
            {
                string? parentEstimateCategoryName = parentEstimateCategories.FirstOrDefault(p => p.Id == data.EstimateCategory.ParentEstimateCategoryId)?.Name ?? "";
                return new ScheduleDataWithTaskMappingDto
                {
                    Id = data.EstimateCategory.Id,
                    Name = data.EstimateCategory.Name,
                    Sequence = data.EstimateCategory.Sequence,
                    ParentEstimateCategoryId = data.EstimateCategory.ParentEstimateCategoryId,
                    Description = data.EstimateCategory.Description,
                    ParentEstimateCategory = parentEstimateCategoryName,
                    Tasks = data.Tasks.Select(t => new TaskMappingDto
                    {
                        Id = t.Id,
                        ConstructionTaskId = t.ConstructionTaskId,
                        EstimateCategoryId = t.EstimateCategoryId, 
						Name = t.ConstructionTask?.Name
                    }).ToList()
                };
            }).ToList();

            return result;
        }


        public async Task<List<ScheduleDataWithTaskMappingDto>> SaveScheduleteDataMappingsAsync(List<TaskMappingDto> scheduleDataMappings)
		{
			// process updated
			var updatedMappings = scheduleDataMappings.Where(e => e.Updated).ToList();
			foreach (var updatedMapping in updatedMappings)
			{
				var dataMapping = ClientDbContext.ScheduleTaskMappings.FirstOrDefault(m => m.Id == updatedMapping.Id);
				if (dataMapping != null)
				{
					ClientDbContext.ScheduleTaskMappings.Remove(dataMapping);
				}
			}

			// process added
			var newMappings = scheduleDataMappings.Where(e => e.Added).ToList();
			foreach (var newMapping in newMappings)
			{
				var dataMapping = new ScheduleTaskMapping
				{
					EstimateCategoryId = newMapping.EstimateCategoryId,
					CreatedBy = this.UserId,
					DateCreated = DateTime.UtcNow,
					ConstructionTaskId = newMapping.ConstructionTaskId,
					Id = Guid.NewGuid()
				};

				await ClientDbContext.ScheduleTaskMappings.AddAsync(dataMapping);
			}

			await ClientDbContext.SaveChangesAsync();
			return await this.GetScheduleDataMappingsAsync();

		}
	}
}
