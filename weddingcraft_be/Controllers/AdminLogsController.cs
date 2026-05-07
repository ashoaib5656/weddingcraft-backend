using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers;

[ApiController]
[Route("api/admin/logs")]
[Authorize(Roles = "Admin")]
public class AdminLogsController : ControllerBase
{
    private readonly ILogService _logService;
    public AdminLogsController(ILogService logService) => _logService = logService;

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] PaginationFilter filter, [FromQuery] string? level = null, [FromQuery] string? endpoint = null, [FromQuery] string? userEmail = null)
    {
        var pagedLogs = await _logService.GetLogsAsync(filter, level, endpoint, userEmail);

        return Ok(pagedLogs);
    }
}
