using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(6)]
        public string Password { get; set; } = null!;

        // Customer registration must include phone
        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Role { get; set; }
    }
}
