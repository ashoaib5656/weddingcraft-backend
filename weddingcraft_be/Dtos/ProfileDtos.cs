using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Dtos
{
    public class UpdateProfileDto
    {
        [Required]
        public string Name { get; set; } = null!;

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? Location { get; set; }
    }

    public class ChangePasswordDto
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required, MinLength(12)]
        public string NewPassword { get; set; } = null!;
    }
}