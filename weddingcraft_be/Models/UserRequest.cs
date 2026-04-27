using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class UserRequest
    {
        public int Id { get; set; }
        
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Category { get; set; } = null!;

        [Required]
        public string Type { get; set; } = "Inquiry"; // Inquiry or Booking

        [Required]
        public string RequestDate { get; set; } = null!;

        public string? Description { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}