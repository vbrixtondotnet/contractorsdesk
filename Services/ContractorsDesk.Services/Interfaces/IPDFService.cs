using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Services.Interfaces.@base;
using DinkToPdf;

namespace ContractorsDesk.Services.Interfaces
{
    public interface IPDFService
	{
		Task<Stream> ConvertHtmlToPdfFromUrl(string url, Orientation orientation = Orientation.Portrait);
		byte[] GetCostPlusPDF(ClientContractDetails clientContractDetails);
	}
}
