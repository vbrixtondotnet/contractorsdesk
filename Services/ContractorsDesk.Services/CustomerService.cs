using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ContractorsDesk.Services
{
	public class CustomerService : BaseService, ICustomerService
	{
		public CustomerService(ClientDbContext clientDataDbContext, IMapper mapper) : 
			base(mapper, clientDataDbContext) 
		{
			this.ClientDbContext = clientDataDbContext;
			this.mapper = mapper;
		}

		public async Task<List<ClientDto>> SearchCustomersAsync(string searchKey)
		{
            var dbCustomers = await ClientDbContext.Clients.OrderBy(c => c.Name).Take(5).ToListAsync();

            if (!string.IsNullOrEmpty(searchKey))
            {
                dbCustomers = await ClientDbContext.Clients
                .Where(c => c.Name.Contains(searchKey) ||
                            c.EmailAddress.Contains(searchKey) ||
                            c.CompanyName.Contains(searchKey))
                .ToListAsync();
            }

            var clients = mapper.Map<List<ClientDto>>(dbCustomers);
            return clients.DistinctBy(c => c.Name).ToList();
        }
	}
}
