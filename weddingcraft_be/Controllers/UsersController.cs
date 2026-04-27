using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UsersController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? role)
        {
            var query = _db.Users.AsNoTracking();
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            var users = await query
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Name = u.Name,
                    Role = u.Role,
                    PhoneNumber = u.PhoneNumber,
                    Status = u.Status,
                    Location = u.Location,
                    Category = u.Category,
                    Department = u.Department,
                    Rating = u.Rating,
                    CreatedAt = u.CreatedAt,
                    LastSeen = u.LastSeen
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                Location = user.Location,
                Category = user.Category,
                Department = user.Department,
                Rating = user.Rating,
                CreatedAt = user.CreatedAt,
                LastSeen = user.LastSeen
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.PhoneNumber = dto.PhoneNumber;
            user.Status = dto.Status;
            user.Location = dto.Location;
            user.Category = dto.Category;
            user.Department = dto.Department;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            return Ok(new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status,
                Location = user.Location,
                Category = user.Category,
                Department = user.Department,
                Rating = user.Rating,
                CreatedAt = user.CreatedAt,
                LastSeen = user.LastSeen
            });
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            user.Name = dto.Name;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Location = dto.Location;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto, [FromServices] Microsoft.AspNetCore.Identity.IPasswordHasher<User> hasher)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _db.Users.FindAsync(userId);
            if (user == null) return NotFound();

            var result = hasher.VerifyHashedPassword(user, user.PasswordHash!, dto.CurrentPassword);
            if (result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Failed)
            {
                return BadRequest(new { message = "Invalid current password" });
            }

            user.PasswordHash = hasher.HashPassword(user, dto.NewPassword);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}