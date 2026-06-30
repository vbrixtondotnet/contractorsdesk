using ContractorsDesk.Core.Models;
using ContractorsDesk.DataStore.Account.Interfaces;
using ContractorsDesk.DataStore.Account.Repositories;

namespace ContractorsDesk.DataStore.Account
{
	public class UnitOfWork : IUnitOfWork
	{
		public IRepository<ApplicationUser> Users { get; }
		public IRepository<ApplicationUserRole> UserRoles { get; }
		public IRepository<ApplicationRole> Roles { get; }
		public IRepository<Permission> Permissions { get; }
		public AccountDbContext Context { get; }
		public UnitOfWork(AccountDbContext context)
		{
			Context = context;
			Users = new Repository<ApplicationUser>(context);
			UserRoles = new Repository<ApplicationUserRole>(context);
			Roles = new Repository<ApplicationRole>(context);
			Permissions = new Repository<Permission>(context);
		}

		public async Task<int> CommitAsync()
		{
			return await Context.SaveChangesAsync();
		}

		public void Dispose()
		{
			Context.Dispose();
		}
	}
}
