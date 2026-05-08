using Microsoft.AspNetCore.SignalR;
using weddingcraft_be.Hubs;

namespace weddingcraft_be.Services;

public interface INotificationService
{
    Task NotifyUserAsync(Guid userId, string title, string message, string type = "info", object? data = null);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyUserAsync(Guid userId, string title, string message, string type = "info", object? data = null)
    {
        await _hubContext.Clients.Group($"User_{userId}").SendAsync("ReceiveNotification", new
        {
            title,
            message,
            type,
            data,
            timestamp = DateTime.UtcNow
        });
    }
}
