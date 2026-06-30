using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IInvoiceService : IBaseService
	{
		Task<InvoiceDto> GetInvoiceAsync(Guid id);
		Task<InvoiceDto> CreateInvoiceAsync(InvoicePayload invoice);
		Task<InvoiceDto> UpdateInvoiceAsync(InvoicePayload invoice);
		Task<string> GenerateInvoiceNumberAsync(Guid clientId);

	}
}
