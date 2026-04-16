namespace weddingcraft_be.Interfaces.Services
{
    public interface IEmailService { 
        Task SendAsync(string to, string subject, string html);
        Task SendOtpAsync(string to, string otp);
    }
}
