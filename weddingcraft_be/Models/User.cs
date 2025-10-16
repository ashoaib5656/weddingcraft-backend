using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        public string Role { get; set; } = "Customer";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Order> Orders { get; set; } = new();

        public List<AiRequest> AiRequests { get; set; } = new();
    }

}
