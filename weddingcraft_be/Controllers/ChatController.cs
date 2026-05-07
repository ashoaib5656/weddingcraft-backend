using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService) => _chatService = chatService;

    [HttpGet("history")]
    public async Task<IActionResult> History([FromQuery] PaginationFilter filter)
    {
        var pagedResponse = await _chatService.GetHistoryAsync(filter);
        return Ok(pagedResponse);
    }
}
