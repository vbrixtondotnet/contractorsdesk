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
	public class InvoiceService : BaseService, IInvoiceService
	{
		public InvoiceService(ClientDbContext clientDbContext, IMapper mapper)
			: base(mapper, clientDbContext)
		{
			
		}
		public async Task<InvoiceDto> GetInvoiceAsync(Guid id)
		{
			var invoice = await ClientDbContext.Invoices
				.Include(i => i.Client)
				.Include(i => i.InvoiceItems)
				.FirstOrDefaultAsync(i => i.Id == id) ?? throw new Exception("Invoice not found.");

			var invoiceDto = mapper.Map<InvoiceDto>(invoice);
			invoiceDto.BillTo = invoice.Client.Name;
			return invoiceDto;
		}

		public async Task<InvoiceDto> CreateInvoiceAsync(InvoicePayload invoice)
		{
			var dbInvoice = mapper.Map<Invoice>(invoice);
			dbInvoice.Id = Guid.NewGuid();
			if(invoice.Items != null)
			{
				foreach(var item in invoice.Items)
				{
					var invoiceItem = mapper.Map<InvoiceItem>(item);
					invoiceItem.Id = Guid.NewGuid();
					invoiceItem.Sequence = dbInvoice.InvoiceItems.Count + 1;
					dbInvoice.InvoiceItems.Add(invoiceItem);
				}
			}

			if(invoice.InvoiceNumber == string.Empty)
			{
				dbInvoice.InvoiceNumber = await this.GenerateInvoiceNumberAsync(invoice.ClientId);
			}

			ClientDbContext.Invoices.Add(dbInvoice);
			await ClientDbContext.SaveChangesAsync();

			return mapper.Map<InvoiceDto>(dbInvoice);
		}

		public async Task<string> GenerateInvoiceNumberAsync(Guid clientId)
		{
			var invoices = await ClientDbContext.Invoices.ToListAsync();

			var invoice = invoices
				.OrderByDescending(i => int.TryParse(i.InvoiceNumber, out var num) ? num : 0)
				.FirstOrDefault();

			var invoiceNumber = invoice == null ? 1 : int.Parse(invoice.InvoiceNumber) + 1;

			return Convert.ToInt32(invoiceNumber).ToString().PadLeft(5,'0');
		}

		public Task<InvoiceDto> UpdateInvoiceAsync(InvoicePayload invoice)
		{
			throw new NotImplementedException();
		}
	}
}
