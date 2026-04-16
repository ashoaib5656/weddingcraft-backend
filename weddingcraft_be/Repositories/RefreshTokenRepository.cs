using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Models;

namespace weddingcraft_be.Repositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(ApplicationDbContext db) : base(db) { }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await _dbSet.Include(r => r.User).SingleOrDefaultAsync(r => r.Token == token);
}
