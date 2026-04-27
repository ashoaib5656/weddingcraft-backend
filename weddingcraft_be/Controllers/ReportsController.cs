using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public ReportsController(ApplicationDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _db.Reports.AsNoTracking().OrderByDescending(r => r.CreatedAt).ToListAsync());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] Report report)
        {
            _db.Reports.Add(report);
            await _db.SaveChangesAsync();
            return Ok(report);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var report = await _db.Reports.FindAsync(id);
            if (report == null) return NotFound();
            _db.Reports.Remove(report);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}