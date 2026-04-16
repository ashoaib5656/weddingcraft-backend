using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Models;

namespace weddingcraft_be.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
}
