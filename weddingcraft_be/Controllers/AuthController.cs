using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using WeddingCraft.Api.Services;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;

namespace WeddingCraft.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IJwtService _jwt;

    public AuthController(ApplicationDbContext db, IPasswordHasher<User> hasher, IJwtService jwt)
    {
        _db = db; _hasher = hasher; _jwt = jwt;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            return BadRequest("Email already registered");

        var user = new User { Email = dto.Email };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(new { token = _jwt.GenerateToken(user) });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return Unauthorized();

        var res = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (res == PasswordVerificationResult.Failed) return Unauthorized();

        return Ok(new { token = _jwt.GenerateToken(user) });
    }
}
