using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        public string? Name { get; set; }

        public string? PasswordHash { get; set; }
        
        [Required]
        public string Role { get; set; } = "Customer";

        public string? PhoneNumber { get; set; }

        public string Status { get; set; } = "Active";

        public string? Location { get; set; }

        public double Rating { get; set; } = 0.0;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? LastSeen { get; set; }

        public VendorProfile? VendorProfile { get; set; }

        public List<Product> Products { get; set; } = new();
        public List<Order> Orders { get; set; } = new();
        public List<AiRequest> AiRequests { get; set; } = new();
    }
}
