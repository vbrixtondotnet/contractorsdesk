using AutoMapper;
using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.Core.Dto;
using ContractorsDesk.Core.Enums;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pipelines.Sockets.Unofficial.Arenas;
using ActionItemPayload = ContractorsDesk.Core.ApiPayloadModels.ActionItemPayload;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class TenantService : BaseService, ITenantService
	{
		public TenantService(MasterDbContext masterDbContext) : base(null, null, masterDbContext)
		{
		}

		public async Task<string> GetConnectionString(string subDomain)
		{
			// Map subdomain to connection string name
			var connectionString = await this.masterDbContext.Companies.Include(c => c.ConnectionStrings).FirstOrDefaultAsync(c => c.SubDomain == subDomain);

			if (connectionString != null && connectionString.ConnectionStrings != null && connectionString.ConnectionStrings.Count > 0)
				return connectionString.ConnectionStrings.First().Value;

			throw new Exception("Connection string not found for subdomain: " + subDomain);
		}
		public async Task<ClientDbContext> GetClientDatabase(string subDomain)
		{
			var connectionString = await this.GetConnectionString(subDomain);

			var optionsBuilder = new DbContextOptionsBuilder<ClientDbContext>();
			optionsBuilder.UseSqlServer(connectionString);

			return new ClientDbContext(optionsBuilder.Options);
		}


	}
}
