using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Dtos;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.RegisterAsync(dto, GetIp());
        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.LoginAsync(dto, GetIp());
        return Ok(result);
    }

    [HttpPost("create-user")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _authService.CreateUserAsync(dto);
        return Ok(new { success = true, role = result.Role });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto req)
    {
        var result = await _authService.RefreshAsync(req.RefreshToken, GetIp());
        return Ok(result);
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> Revoke([FromBody] RefreshRequestDto req)
    {
        await _authService.RevokeAsync(req.RefreshToken, GetIp());
        return NoContent();
    }

    [HttpPost("send-otp")]
    public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
    {
        await _authService.SendOtpAsync(request.Email);
        return Ok(new { message = "OTP sent successfully." });
    }

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        await _authService.VerifyOtpAsync(request.Email, request.Otp);
        return Ok(new { message = "OTP verified successfully." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] SendOtpRequest request)
    {
        await _authService.SendPasswordResetOtpAsync(request.Email);
        return Ok(new { message = "Password reset OTP sent successfully." });
    }

    [HttpPost("verify-password-reset-otp")]
    public async Task<IActionResult> VerifyPasswordResetOtp([FromBody] VerifyOtpRequest request)
    {
        await _authService.VerifyPasswordResetOtpAsync(request.Email, request.Otp);
        return Ok(new { message = "OTP verified successfully." });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request.Email, request.NewPassword);
        return Ok(new { message = "Password reset successfully." });
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    private string GetIp() =>
        HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
