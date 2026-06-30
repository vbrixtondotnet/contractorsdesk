using ContractorsDesk.Core.Models;

namespace ContractorsDesk.DataStore.Account.Interfaces
{
	public interface IUnitOfWork : IDisposable
	{
		AccountDbContext Context { get; }
		IRepository<ApplicationUser> Users { get; }
		IRepository<ApplicationUserRole> UserRoles { get; }
		IRepository<ApplicationRole> Roles { get; }
		IRepository<Permission> Permissions { get; }
		Task<int> CommitAsync(); // Save changes to the database
	}
}
