using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using ContractorsDesk.Core.Enums;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.Core.ApiPayloadModels;
using Pipelines.Sockets.Unofficial.Arenas;
using MimeKit.Utils;
using ContractorsDesk.Core.Utilities;
using DocuSign.eSign.Model;

namespace ContractorsDesk.Services
{
	public class ChangeOrderService : BaseService, IChangeOrderService
	{
		public ChangeOrderService(
			ClientDbContext clientDbContext,
			IMapper mapper)
			: base(mapper, clientDbContext)
		{
		}
		public async Task<ChangeOrderDto> CreateChangeOrderAsync(ChangeOrderDto changeOrderDto)
		{
			var changeOrderNumber = ClientDbContext.ChangeOrders
				.Max(co => co.ChangeOrderNumber);

			changeOrderNumber = changeOrderNumber == null ? 1 : changeOrderNumber + 1;

			var changeOrder = this.mapper.Map<ChangeOrder>(changeOrderDto);

			changeOrder.Id = Guid.NewGuid();
			changeOrder.ChangeOrderNumber = changeOrderNumber;
			ClientDbContext.ChangeOrders.Add(changeOrder);
			await ClientDbContext.SaveChangesAsync();

			return this.mapper.Map<ChangeOrderDto>(changeOrder);

		}

		public async Task<ChangeOrderDto> GetChangeOrderByActionItemIdAsync(int actionItemId)
		{
			var changeOrder = await ClientDbContext.ChangeOrders
				.AsNoTracking()
				.FirstOrDefaultAsync(co => co.ActionItemId == actionItemId) ?? throw new Exception("Change Order does not exist.");

			return this.mapper.Map<ChangeOrderDto>(changeOrder);
		}

		public Task<ChangeOrderDto> GetChangeOrderByIdAsync(int id)
		{
			throw new NotImplementedException();
		}
	}
}
