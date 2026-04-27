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
    public class UserRequestsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public UserRequestsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var query = _db.UserRequests.Include(r => r.User).AsNoTracking();

            if (role == "Customer" || role == "Client")
            {
                query = query.Where(r => r.UserId == userId);
            }

            var requests = await query
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new UserRequestDto
                {
                    Id = r.Id,
                    Subject = r.Subject,
                    Category = r.Category,
                    Type = r.Type,
                    RequestDate = r.RequestDate,
                    Description = r.Description,
                    Status = r.Status,
                    SenderName = r.User.Name ?? r.User.Email
                })
                .ToListAsync();

            return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserRequestDto dto)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var request = new UserRequest
            {
                UserId = userId,
                Subject = dto.Subject,
                Category = dto.Category,
                Type = dto.Type,
                RequestDate = dto.RequestDate,
                Description = dto.Description,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            _db.UserRequests.Add(request);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Request submitted successfully" });
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var request = await _db.UserRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = status;
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}