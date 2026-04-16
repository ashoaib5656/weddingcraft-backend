using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Hubs;

[Authorize] // require authenticated users (optional)
public class ChatHub : Hub
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpCtx;

    public ChatHub(ApplicationDbContext db, IHttpContextAccessor httpCtx)
    {
        _db = db;
        _httpCtx = httpCtx;
    }

    // Called by clients to send message
    public async Task SendMessage(string message)
    {
        // Optional: get user info from Claims
        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = null;
        if (Guid.TryParse(userIdClaim, out var parsed)) userId = parsed;
        var email = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var chat = new ChatMessage
        {
            UserId = userId,
            UserEmail = email,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        _db.ChatMessages.Add(chat);
        await _db.SaveChangesAsync();

        // Broadcast to all clients (or a group)
        await Clients.All.SendAsync("ReceiveMessage", new
        {
            id = chat.Id,
            userId = chat.UserId,
            userEmail = chat.UserEmail,
            message = chat.Message,
            createdAt = chat.CreatedAt
        });
    }
}
