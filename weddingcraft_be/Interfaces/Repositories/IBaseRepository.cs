using System.Linq.Expressions;

namespace weddingcraft_be.Interfaces.Repositories;

public interface IBaseRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    
    // Advanced Querying
    IQueryable<T> GetQueryable();
    
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
    
    Task SaveChangesAsync();
}
