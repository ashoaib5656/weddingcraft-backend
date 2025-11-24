using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;
using weddingcraft_be.Services;

namespace weddingcraft_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IJwtService _jwt;
    private readonly IHttpContextAccessor _httpCtx;

    public AuthController(ApplicationDbContext db, IPasswordHasher<User> hasher, IJwtService jwt, IHttpContextAccessor httpCtx)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
        _httpCtx = httpCtx;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email)) return BadRequest(new { error = "Email already registered" });

        var user = new User { Email = dto.Email, Role = "Customer" };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        var access = _jwt.GenerateAccessToken(user);
        var (refreshToken, refreshExpires) = _jwt.GenerateRefreshToken();

        var rt = new RefreshToken { Token = refreshToken, UserId = user.Id, ExpiresAt = refreshExpires, CreatedByIp = GetIp() };
        _db.RefreshTokens.Add(rt);
        await _db.SaveChangesAsync();

        return Ok(new AuthResultDto { AccessToken = access, RefreshToken = refreshToken, ExpiresAt = DateTime.UtcNow.AddMinutes(_jwt is null ? 15 : 15) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Unauthorized();

        var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (res == PasswordVerificationResult.Failed) return Unauthorized();

        var access = _jwt.GenerateAccessToken(user);
        var (refreshToken, refreshExpires) = _jwt.GenerateRefreshToken();

        var rt = new RefreshToken { Token = refreshToken, UserId = user.Id, ExpiresAt = refreshExpires, CreatedByIp = GetIp() };
        _db.RefreshTokens.Add(rt);
        await _db.SaveChangesAsync();

        return Ok(new AuthResultDto { AccessToken = access, RefreshToken = refreshToken, ExpiresAt = DateTime.UtcNow.AddMinutes(15) });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto req)
    {
        var stored = await _db.RefreshTokens.Include(r => r.User).SingleOrDefaultAsync(r => r.Token == req.RefreshToken);
        if (stored == null || !stored.IsActive) return Unauthorized();

        stored.RevokedAt = DateTime.UtcNow;
        stored.RevokedByIp = GetIp();

        var (newToken, newExpires) = _jwt.GenerateRefreshToken();
        stored.ReplacedByToken = newToken;
        _db.RefreshTokens.Add(new RefreshToken
        {
            Token = newToken,
            UserId = stored.UserId,
            ExpiresAt = newExpires,
            CreatedByIp = GetIp()
        });

        await _db.SaveChangesAsync();

        var access = _jwt.GenerateAccessToken(stored.User);
        return Ok(new AuthResultDto { AccessToken = access, RefreshToken = newToken, ExpiresAt = DateTime.UtcNow.AddMinutes(15) });
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto req)
    {
        var stored = await _db.RefreshTokens.SingleOrDefaultAsync(r => r.Token == req.RefreshToken);
        if (stored == null) return NotFound();
        stored.RevokedAt = DateTime.UtcNow;
        stored.RevokedByIp = GetIp();
        await _db.SaveChangesAsync();
        return NoContent();
    }

    private string GetIp()
    {
        return _httpCtx.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
    }
}
