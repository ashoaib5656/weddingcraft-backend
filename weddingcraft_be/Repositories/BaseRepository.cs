using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using weddingcraft_be.Data;
using weddingcraft_be.Interfaces.Repositories;

namespace weddingcraft_be.Repositories;

public abstract class BaseRepository<T> : IBaseRepository<T> where T : class
{
    protected readonly ApplicationDbContext _db;
    protected readonly DbSet<T> _dbSet;

    protected BaseRepository(ApplicationDbContext db)
    {
        _db = db;
        _dbSet = db.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) 
        => await _dbSet.Where(predicate).ToListAsync();

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate) 
        => await _dbSet.SingleOrDefaultAsync(predicate);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) 
        => await _dbSet.AnyAsync(predicate);

    public IQueryable<T> GetQueryable() => _dbSet.AsQueryable();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public async Task SaveChangesAsync()
    {
        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("A database error occurred while saving changes.", ex);
        }
        catch (Exception ex)
        {
            throw new Exception("An unexpected error occurred in the database layer.", ex);
        }
    }

}
