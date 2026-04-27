using Microsoft.Extensions.Options;
using weddingcraft_be.Models.Configuration;
using MailKit.Net.Smtp;
using MimeKit;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;
    public EmailService(IOptions<SmtpSettings> options) => _settings = options.Value;

    public async Task SendAsync(string to, string subject, string html)
    {
        var msg = new MimeMessage();
        msg.From.Add(MailboxAddress.Parse(_settings.From));
        msg.To.Add(MailboxAddress.Parse(to));
        msg.Subject = subject;
        msg.Body = new TextPart("html") { Text = html };

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, false);
        if (!string.IsNullOrEmpty(_settings.User))
            await client.AuthenticateAsync(_settings.User, _settings.Pass);
        await client.SendAsync(msg);
        await client.DisconnectAsync(true);
    }

    public async Task SendOtpAsync(string to, string otp)
    {
        var subject = "Your WeddsPot OTP Code";
        var html = $@"
                <div style='font-family: Arial, sans-serif'>
                    <h2>WeddsPot Verification</h2>
                    <p>Your One-Time Password (OTP) is:</p>
                    <h1 style='letter-spacing: 4px'>{otp}</h1>
                    <p>This OTP is valid for <b>5 minutes</b>.</p>
                    <p>If you did not request this, please ignore this email.</p>
                </div>";

        await SendAsync(to, subject, html);

    }
}
