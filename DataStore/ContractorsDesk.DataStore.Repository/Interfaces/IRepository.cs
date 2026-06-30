using System.Linq.Expressions;

namespace ContractorsDesk.DataStore.Account.Interfaces
{
	public interface IRepository<T> where T : class
	{
		IQueryable<T> Include(Expression<Func<T, object>> includeExpression);
		IQueryable<T> Where(Expression<Func<T, bool>> includeExpression);
		Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);
		Task<T> GetByIdAsync(int id);
		Task<IEnumerable<T>> GetAllAsync();
		Task AddAsync(T entity);
		void Update(T entity);
		void Delete(T entity);
	}
}
