using ContractorsDesk.DataStore.Account.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ContractorsDesk.DataStore.Account.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly DbContext _context;

		public Repository(DbContext context)
		{
			_context = context;
		}

		public async Task<T> GetByIdAsync(int id)
		{
			return await _context.Set<T>().FindAsync(id);
		}

		public IQueryable<T> All()
		{
			return _context.Set<T>().AsQueryable();
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _context.Set<T>().ToListAsync();
		}

		public async Task AddAsync(T entity)
		{
			await _context.Set<T>().AddAsync(entity);
		}

		public void Update(T entity)
		{
			_context.Set<T>().Update(entity);
		}

		public void Delete(T entity)
		{
			_context.Set<T>().Remove(entity);
		}

		public IQueryable<T> Include(Expression<Func<T, object>> includeExpression)
		{
			return _context.Set<T>().Include(includeExpression);
		}

		public IQueryable<T> Where(Expression<Func<T, bool>> expression)
		{
			return _context.Set<T>().Where(expression);
		}
		public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression)
		{
			return await _context.Set<T>().FirstOrDefaultAsync(expression);
		}




		//public IQueryable<T> ThenInclude(params Expression<Func<T, object>>[] includeProperties)
		//{
		//	IQueryable<T> query = _context.Set<T>();
		//	foreach (var includeProperty in includeProperties)
		//	{
		//		query = query.ThenInclude(includeProperty);
		//	}
		//	return query;
		//}
	}
}
