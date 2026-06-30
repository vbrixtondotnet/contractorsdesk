using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.Interfaces.@base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IVendorService : IBaseService
	{
        Task<List<VendorDto>> GetVendorsAsync(string? searchKey = null);
		Task<VendorDto> AddVendorAsync(VendorPayload model);
		Task<bool> DeleteVendorAsync(Guid id);
		Task<List<string>> GetVendorTypesAsync();
    }
}
