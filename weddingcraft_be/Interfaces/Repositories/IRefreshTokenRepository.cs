using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Models;

namespace weddingcraft_be.Interfaces.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}
