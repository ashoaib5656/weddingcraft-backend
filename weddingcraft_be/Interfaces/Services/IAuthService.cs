using weddingcraft_be.Dtos;

namespace weddingcraft_be.Interfaces.Services;

public interface IAuthService
{
    // ─── Registration & Login ─────────────────────────────────────────────────
    Task<AuthResultDto> RegisterAsync(RegisterDto dto, string ipAddress);
    Task<AuthResultDto> LoginAsync(LoginDto dto, string ipAddress);
    Task<AuthResultDto> RefreshAsync(string refreshToken, string ipAddress);
    Task RevokeAsync(string refreshToken, string ipAddress);
    Task<AuthResultDto> CreateUserAsync(CreateUserDto dto);

    // ─── OTP ─────────────────────────────────────────────────────────────────
    Task SendOtpAsync(string email);
    Task VerifyOtpAsync(string email, string otp);

    // ─── Password Reset ───────────────────────────────────────────────────────
    Task SendPasswordResetOtpAsync(string email);
    Task VerifyPasswordResetOtpAsync(string email, string otp);
    Task ResetPasswordAsync(string email, string newPassword);
}
