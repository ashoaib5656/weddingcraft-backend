using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Models;

namespace weddingcraft_be.Repositories;

public class UserRepository : BaseRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext db) : base(db) { }

    public async Task<User?> GetByEmailAsync(string email) 
        => await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
}
