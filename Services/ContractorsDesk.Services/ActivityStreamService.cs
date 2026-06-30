using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.ApiPayloadModels;

namespace ContractorsDesk.Services
{
	public class ActivityStreamService : BaseService, IActivityStreamService
	{
		#region Public Methods
		public ActivityStreamService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext) 
		{

		}
		public async Task CreateActivityStream(ActivityStreamPayload activityStreamPayload)
		{
			foreach (var item in activityStreamPayload.Activity)
			{
				var activityStream = mapper.Map<ActivityStream>(item);

				ClientDbContext.Entry(activityStream).State = EntityState.Added;
				await ClientDbContext.ActivityStreams.AddAsync(activityStream);
			}

			await ClientDbContext.SaveChangesAsync();
		}

		#endregion

		#region Private Methods

		#endregion
	}
}
