using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.DataStore.Client.StoredProcedures.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using ContractorsDesk.DataStore.Master.Models;

namespace ContractorsDesk.Services
{
	public class QbAccountService : BaseService, IQbAccountService
	{
		public QbAccountService(
			ClientDbContext clientDataDbContext, 
			MasterDbContext masterDbContext,
			IMapper mapper,
			IConfiguration configuration)
			:base(mapper, clientDataDbContext, masterDbContext, configuration) {}

		public async Task<List<QBAccountDto>> GetQbAccountsAsync()
		{
			return await ClientDbContext.Qbaccounts
				.Where(a => a.AccountType == "Expense")
				.Select(a => mapper.Map<QBAccountDto>(a))
				.ToListAsync();
		}
	}
}
