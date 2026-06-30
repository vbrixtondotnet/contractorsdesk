using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ConstructionTaskDto = ContractorsDesk.Core.Dto.ConstructionTaskDto;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.Services
{
	public class ConstructionTasksService : BaseService, IConstructionTasksService
	{
		public ConstructionTasksService(ClientDbContext clientDataDbContext, IMapper mapper)
			: base(mapper, clientDataDbContext) 
		{

		}

		#region Public Methods

		public async Task<List<ConstructionTaskDto>> GetAllConstructionTaskAsync()
		{
			var constructionTasks = await ClientDbContext.ConstructionTasks
				.Include(ct=> ct.ParentTask)
				.OrderBy(ct=> ct.Sequence)
				.ToListAsync();

			return mapper.Map<List<ConstructionTaskDto>>(constructionTasks);
		}

		public async Task<ConstructionTaskDto> CreateConstructionTaskAsync(ConstructionTaskPayload payload)
		{

			var dbConstructionTask = new ConstructionTask()
			{
				Id = Guid.NewGuid(),
				Name = payload.Name,
				ParentTaskId = payload.ParentTaskId,
				Sequence = payload.Sequence,
				Duration = payload.Duration,
				Pred1Lag = 0
			};

			ClientDbContext.ConstructionTasks.Add(dbConstructionTask);
			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<ConstructionTaskDto>(dbConstructionTask);
		}
		#endregion

		#region Private Methods

		#endregion
	}
}
