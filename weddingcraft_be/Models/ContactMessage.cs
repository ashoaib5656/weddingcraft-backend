using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class ContactMessage
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Subject { get; set; } = null!;
        [Required]
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
    }
}