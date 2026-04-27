namespace weddingcraft_be.Dtos;

public class AuthResultDto
{
    public bool Ok { get; set; } = true;
    public string Message { get; set; } = "Success";
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public string Role { get; set; } = null!;
    public string? Name { get; set; }
    public DateTime ExpiresAt { get; set; }
}
