using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using weddingcraft_be.Dtos;
using weddingcraft_be.Exceptions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Models;
using weddingcraft_be.Common.Helpers;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Services;

public class AuthService : IAuthService
{
    private const int MaxOtpAttempts = 3;
    private static readonly TimeSpan OtpWindow = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan OtpAttemptWindow = TimeSpan.FromMinutes(3);

    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _tokenRepo;
    private readonly IPasswordHasher<User> _hasher;
    private readonly IJwtService _jwt;
    private readonly IRedisService _redis;
    private readonly IEmailService _emailService;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepo,
        IRefreshTokenRepository tokenRepo,
        IPasswordHasher<User> hasher,
        IJwtService jwt,
        IRedisService redis,
        IEmailService emailService,
        IOptions<JwtSettings> jwtOptions)
    {
        _userRepo = userRepo;
        _tokenRepo = tokenRepo;
        _hasher = hasher;
        _jwt = jwt;
        _redis = redis;
        _emailService = emailService;
        _jwtSettings = jwtOptions.Value;
    }

    // ─── Register ─────────────────────────────────────────────────────────────

    public async Task<AuthResultDto> RegisterAsync(RegisterDto dto, string ipAddress)
    {
        if (await _userRepo.AnyAsync(u => u.Email == dto.Email))
            throw new ConflictException("Email is already registered.");

        var user = new User { Email = dto.Email, Role = dto.Role ?? "Customer", PhoneNumber = dto.PhoneNumber };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        return await IssueTokensAsync(user, ipAddress);
    }

    // ─── Login ────────────────────────────────────────────────────────────────

    public async Task<AuthResultDto> LoginAsync(LoginDto dto, string ipAddress)
    {
        var user = await _userRepo.GetByEmailAsync(dto.Email)
            ?? throw new BadRequestException("Invalid credentials.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, dto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new BadRequestException("Invalid credentials.");

        return await IssueTokensAsync(user, ipAddress);
    }

    // ─── Refresh ──────────────────────────────────────────────────────────────

    public async Task<AuthResultDto> RefreshAsync(string refreshToken, string ipAddress)
    {
        var stored = await _tokenRepo.GetByTokenAsync(refreshToken)
            ?? throw new BadRequestException("Invalid refresh token.");

        if (!stored.IsActive)
            throw new BadRequestException("Refresh token is expired or revoked.");

        // Rotate
        stored.RevokedAt = DateTime.UtcNow;
        stored.RevokedByIp = ipAddress;

        var (newToken, newExpires) = _jwt.GenerateRefreshToken();
        stored.ReplacedByToken = newToken;
        
        await _tokenRepo.AddAsync(new RefreshToken
        {
            Token = newToken,
            UserId = stored.UserId,
            ExpiresAt = newExpires,
            CreatedByIp = ipAddress
        });

        await _tokenRepo.SaveChangesAsync();

        var access = _jwt.GenerateAccessToken(stored.User);
        return new AuthResultDto
        {
            AccessToken = access,
            RefreshToken = newToken,
            Role = stored.User.Role,
            Name = stored.User.Email.Split('@')[0],
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes)
        };
    }

    // ─── Revoke ───────────────────────────────────────────────────────────────

    public async Task RevokeAsync(string refreshToken, string ipAddress)
    {
        var stored = await _tokenRepo.GetByTokenAsync(refreshToken)
            ?? throw new NotFoundException("Refresh token not found.");

        stored.RevokedAt = DateTime.UtcNow;
        stored.RevokedByIp = ipAddress;
        await _tokenRepo.SaveChangesAsync();
    }

    // ─── Admin Create User ────────────────────────────────────────────────────

    public async Task<AuthResultDto> CreateUserAsync(CreateUserDto dto)
    {
        if (await _userRepo.AnyAsync(u => u.Email == dto.Email))
            throw new ConflictException("Email is already registered.");

        var user = new User 
        { 
            Email = dto.Email, 
            Role = dto.Role, 
            PhoneNumber = dto.PhoneNumber,
            Name = dto.Name,
            Status = dto.Status ? "Active" : "Inactive"
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);
        
        await _userRepo.AddAsync(user);
        await _userRepo.SaveChangesAsync();

        return new AuthResultDto 
        { 
            AccessToken = string.Empty, 
            RefreshToken = string.Empty, 
            Role = user.Role, 
            Name = user.Name,
            ExpiresAt = DateTime.MinValue 
        };
    }

    public async Task<AuthResultDto> VerifyTokenAsync(string token)
    {
        var principal = _jwt.ValidateToken(token)
            ?? throw new BadRequestException("Invalid token.");

        var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? throw new BadRequestException("Invalid token claims.");

        var user = await _userRepo.GetByEmailAsync(email)
            ?? throw new NotFoundException("User not found.");

        return new AuthResultDto
        {
            Ok = true,
            Message = "Token is valid",
            AccessToken = token,
            RefreshToken = string.Empty,
            Role = user.Role,
            Name = user.Email.Split('@')[0],
            ExpiresAt = DateTime.UtcNow // Placeholder as we don't easily have the original expiry without parsing more
        };
    }

    // ─── OTP ─────────────────────────────────────────────────────────────────

    public async Task SendOtpAsync(string email)
    {
        var otpKey = $"auth:otp:{email}";
        var attemptsKey = $"auth:otp_attempts:{email}";

        await _redis.RemoveAsync(attemptsKey);

        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        await _redis.SetAsync(otpKey, otp, OtpWindow);
        await _emailService.SendOtpAsync(email, otp);
    }

    public async Task VerifyOtpAsync(string email, string otp)
    {
        var otpKey = $"auth:otp:{email}";
        var attemptsKey = $"auth:otp_attempts:{email}";

        var attemptsValue = await _redis.GetAsync(attemptsKey);
        var attempts = attemptsValue is null ? 0 : int.Parse(attemptsValue);

        if (attempts >= MaxOtpAttempts)
            throw new BadRequestException("Maximum OTP verification attempts exceeded.");

        var storedOtp = await _redis.GetAsync(otpKey)
            ?? throw new BadRequestException("OTP expired or not found.");

        if (storedOtp != otp)
        {
            await _redis.SetAsync(attemptsKey, (attempts + 1).ToString(), OtpAttemptWindow);
            throw new BadRequestException("Invalid OTP.");
        }

        await _redis.RemoveAsync(otpKey);
        await _redis.RemoveAsync(attemptsKey);
    }

    // ─── Password Reset ───────────────────────────────────────────────────────

    public async Task SendPasswordResetOtpAsync(string email)
    {
        var otpKey = $"auth:password_reset_otp:{email}";
        var attemptsKey = $"auth:password_reset_otp_attempts:{email}";

        await _redis.RemoveAsync(attemptsKey);

        var otp = RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
        await _redis.SetAsync(otpKey, otp, TimeSpan.FromMinutes(15));

        var subject = "Your WeddingCraft Password Reset OTP";
        var html = $@"<h2>WeddingCraft</h2><p>Your Password Reset OTP: <b>{otp}</b></p>";

        await _emailService.SendAsync(email, subject, html);
    }

    public async Task VerifyPasswordResetOtpAsync(string email, string otp)
    {
        var otpKey = $"auth:password_reset_otp:{email}";
        var attemptsKey = $"auth:password_reset_otp_attempts:{email}";

        var attemptsValue = await _redis.GetAsync(attemptsKey);
        var attempts = attemptsValue is null ? 0 : int.Parse(attemptsValue);

        if (attempts >= MaxOtpAttempts)
            throw new BadRequestException("Too many attempts.");

        var storedOtp = await _redis.GetAsync(otpKey)
            ?? throw new BadRequestException("OTP expired or not found.");

        if (storedOtp != otp)
        {
            await _redis.SetAsync(attemptsKey, (attempts + 1).ToString(), OtpAttemptWindow);
            throw new BadRequestException("Invalid OTP.");
        }

        await _redis.SetAsync($"auth:pwdreset_verified:{email}", "true", TimeSpan.FromMinutes(10));
        await _redis.RemoveAsync(otpKey);
        await _redis.RemoveAsync(attemptsKey);
    }

    public async Task ResetPasswordAsync(string email, string newPassword)
    {
        var verifiedKey = $"auth:pwdreset_verified:{email}";
        var isVerified = await _redis.GetAsync(verifiedKey);

        if (isVerified != "true")
            throw new BadRequestException("OTP not verified.");

        var user = await _userRepo.GetByEmailAsync(email)
            ?? throw new NotFoundException("User not found.");

        user.PasswordHash = _hasher.HashPassword(user, newPassword);
        await _userRepo.SaveChangesAsync();
        await _redis.RemoveAsync(verifiedKey);
    }

    // ─── Helper ───────────────────────────────────────────────────────────────

    private async Task<AuthResultDto> IssueTokensAsync(User user, string ipAddress)
    {
        var access = _jwt.GenerateAccessToken(user);
        var (refreshToken, refreshExpires) = _jwt.GenerateRefreshToken();

        await _tokenRepo.AddAsync(new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiresAt = refreshExpires,
            CreatedByIp = ipAddress
        });
        await _tokenRepo.SaveChangesAsync();

        return new AuthResultDto
        {
            AccessToken = access,
            RefreshToken = refreshToken,
            Role = user.Role,
            Name = user.Email.Split('@')[0],
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenMinutes)
        };
    }
}
