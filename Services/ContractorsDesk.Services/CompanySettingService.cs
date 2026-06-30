using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class CompanySettingService : BaseService, ICompanySettingService
	{
		public CompanySettingService(ClientDbContext clientDbContext, MasterDbContext masterDbContext, IMapper mapper)
			: base(mapper, clientDbContext, masterDbContext)
		{
			this.mapper = mapper;
		}

		public async Task<CompanySettingDto> GetCompanySettingAsync()
		{
			var companySetting = await ClientDbContext.CompanySettings
				.AsNoTracking()
				.FirstOrDefaultAsync();

			if (companySetting == null) throw new Exception("Company Settings not found");

			var retval =  mapper.Map<CompanySettingDto>(companySetting);

			var quickbooksSettings = await ClientDbContext.QuickBooksTokens
				.AsNoTracking()
				.FirstOrDefaultAsync();

			retval.QuickbooksConnected = quickbooksSettings != null && !string.IsNullOrEmpty(quickbooksSettings.AccessToken);
			return retval;

		}

		public async Task<CompanySettingDto> SaveCompanySettingAsync(CompanySettingDto companySettingDto)
		{
			var companySetting = await ClientDbContext.CompanySettings
				.FirstOrDefaultAsync();

            if (companySetting == null) throw new Exception("Company Settings not found");

            mapper.Map(companySettingDto, companySetting);
            ClientDbContext.CompanySettings.Update(companySetting);
            await ClientDbContext.SaveChangesAsync();

            return mapper.Map<CompanySettingDto>(companySetting);
        }

		public async Task<CompanySettingDto> UpdateCompanySettingAsync(CompanySettingPayload payload)
		{
            var companySetting = await ClientDbContext.CompanySettings
                .FirstOrDefaultAsync();

			if (companySetting == null)
			{
                companySetting = mapper.Map<CompanySetting>(payload);
                await ClientDbContext.CompanySettings.AddAsync(companySetting);
            }
			else {
                companySetting.CompanyName = payload.CompanyName;
				companySetting.CompanyEmail = payload.CompanyEmail;
				companySetting.GeneralContractorName = payload.GeneralContractorName;
				companySetting.Address1 = payload.Address1;
				companySetting.Address2 = payload.Address2;
				companySetting.PhoneNumber = payload.PhoneNumber;
				companySetting.City = payload.City;
				companySetting.State = payload.State;
				companySetting.Zip = payload.Zip;	
                ClientDbContext.CompanySettings.Update(companySetting);
            }
            await ClientDbContext.SaveChangesAsync();
            return mapper.Map<CompanySettingDto>(companySetting);
        }
	}
}
