using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers;

[ApiController]
[Route("api/admin/logs")]
[Authorize(Roles = "Admin")]
public class AdminLogsController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public AdminLogsController(ApplicationDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get(string? level = null, string? endpoint = null, string? userEmail = null, int page = 1, int pageSize = 25)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 25;

        var q = _db.Logs.AsQueryable();

        if (!string.IsNullOrEmpty(level))
            q = q.Where(l => l.Level == level);

        if (!string.IsNullOrEmpty(endpoint))
            q = q.Where(l => EF.Functions.ILike(l.Endpoint ?? "", $"%{endpoint}%"));

        if (!string.IsNullOrEmpty(userEmail))
            q = q.Where(l => l.UserEmail == userEmail);

        var total = await q.CountAsync();
        var items = await q
            .OrderByDescending(l => l.Timestamp)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new {
                l.Id,
                l.Timestamp,
                l.Level,
                l.Message,
                l.Exception,
                l.UserEmail,
                l.Endpoint,
                l.IpAddress
            })
            .ToListAsync();

        return Ok(new { total, items });
    }
}
