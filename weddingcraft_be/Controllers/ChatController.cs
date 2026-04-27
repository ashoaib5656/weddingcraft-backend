using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;

namespace weddingcraft_be.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public ChatController(ApplicationDbContext db) => _db = db;

    [HttpGet("history")]
    public async Task<IActionResult> History(int page = 1, int pageSize = 50)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 1;
        if (pageSize > 100) pageSize = 100;

        var q = _db.ChatMessages.AsNoTracking().OrderByDescending(m => m.CreatedAt);
        var total = await q.CountAsync();
        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new
            {
                m.Id,
                m.UserId,
                m.UserEmail,
                m.ConversationId,
                m.Message,
                m.CreatedAt
            })
            .ToListAsync();

        items.Reverse(); // oldest-first for UI
        return Ok(new { total, items });
    }
}
