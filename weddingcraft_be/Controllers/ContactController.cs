using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ContactController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessage message)
        {
            _db.ContactMessages.Add(message);
            await _db.SaveChangesAsync();
            return Ok(new { message = "Message sent successfully" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.ContactMessages.AsNoTracking().OrderByDescending(m => m.CreatedAt).ToListAsync());
        }

        [HttpPut("{id}/read")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var msg = await _db.ContactMessages.FindAsync(id);
            if (msg == null) return NotFound();
            msg.IsRead = true;
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}