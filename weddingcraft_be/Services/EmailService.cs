using MailKit.Net.Smtp;
using MimeKit;

namespace weddingcraft_be.Services;

public interface IEmailService { Task SendAsync(string to, string subject, string html); }

public class EmailService : IEmailService
{
    private readonly IConfiguration _cfg;
    public EmailService(IConfiguration cfg) => _cfg = cfg;

    public async Task SendAsync(string to, string subject, string html)
    {
        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(_cfg["Smtp:From"] ?? "no-reply@example.com"));
        msg.To.Add(MailboxAddress.Parse(to));
        msg.Subject = subject;
        msg.Body = new TextPart("html") { Text = html };

        using var client = new SmtpClient();
        await client.ConnectAsync(_cfg["Smtp:Host"], int.Parse(_cfg["Smtp:Port"] ?? "587"), false);
        if (!string.IsNullOrEmpty(_cfg["Smtp:User"]))
            await client.AuthenticateAsync(_cfg["Smtp:User"], _cfg["Smtp:Pass"]);
        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }
}
