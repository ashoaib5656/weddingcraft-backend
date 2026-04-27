using System.Security.Claims;
using weddingcraft_be.Models;

namespace weddingcraft_be.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        (string token, DateTime expiresAt) GenerateRefreshToken();
        ClaimsPrincipal? ValidateToken(string token);
    }
}
