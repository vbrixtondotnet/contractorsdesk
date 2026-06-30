using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractorsDesk.Services.Interfaces
{
	public interface ICompanySettingService
	{
		Task<CompanySettingDto> GetCompanySettingAsync();
		Task<CompanySettingDto> SaveCompanySettingAsync(CompanySettingDto companySettingDto);
		Task<CompanySettingDto> UpdateCompanySettingAsync(CompanySettingPayload payload);

    }
}
