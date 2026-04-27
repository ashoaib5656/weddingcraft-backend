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

    // Called by clients to join a specific conversation room
    public async Task JoinRoom(string conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, conversationId);
        
        var email = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            await Clients.Group(conversationId).SendAsync("UserJoined", email);
        }
    }

    public async Task LeaveRoom(string conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, conversationId);
    }

    // Called by clients to send message
    public async Task SendMessage(string message, string conversationId)
    {
        if (string.IsNullOrEmpty(conversationId)) return;

        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        Guid? userId = null;
        if (Guid.TryParse(userIdClaim, out var parsed)) userId = parsed;
        var email = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

        var chat = new ChatMessage
        {
            UserId = userId,
            UserEmail = email,
            ConversationId = conversationId,
            Message = message,
            CreatedAt = DateTime.UtcNow
        };

        _db.ChatMessages.Add(chat);
        await _db.SaveChangesAsync();

        // Broadcast ONLY to the specific conversation group
        await Clients.Group(conversationId).SendAsync("ReceiveMessage", new
        {
            id = chat.Id,
            userId = chat.UserId,
            userEmail = chat.UserEmail,
            conversationId = chat.ConversationId,
            message = chat.Message,
            createdAt = chat.CreatedAt
        });
    }

    public async Task SendTyping(string conversationId)
    {
        var email = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(conversationId))
        {
            await Clients.Group(conversationId).SendAsync("UserTyping", email);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var email = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            await Clients.All.SendAsync("UserPresence", email, true);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var email = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        if (!string.IsNullOrEmpty(email))
        {
            await Clients.All.SendAsync("UserPresence", email, false);
        }
        await base.OnDisconnectedAsync(exception);
    }
}
