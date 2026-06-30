using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
    public interface ITenantService
	{
		public Task<string> GetConnectionString(string subDomain);
		Task<ClientDbContext> GetClientDatabase(string subDomain);
	}
}
