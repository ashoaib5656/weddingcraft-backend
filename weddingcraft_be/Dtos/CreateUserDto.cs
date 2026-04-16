using System.ComponentModel.DataAnnotations;

public class CreateUserDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;

    [Required]
    [RegularExpression("Admin|Manager|Vendor|Staff|Customer", ErrorMessage = "Role must be one of Admin, Manager, Vendor, Staff, Customer")]
    public string Role { get; set; } = null!;

    [Phone]
    public string? PhoneNumber { get; set; } // optional for non-customer created by admin
}
