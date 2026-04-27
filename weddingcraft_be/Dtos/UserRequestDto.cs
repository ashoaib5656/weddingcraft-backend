using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Dtos
{
    public class UserRequestDto
    {
        public int Id { get; set; }

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Category { get; set; } = null!;

        [Required]
        public string Type { get; set; } = "Inquiry";

        [Required]
        public string RequestDate { get; set; } = null!;

        public string? Description { get; set; }

        public string Status { get; set; } = "Pending";
        
        public string? SenderName { get; set; }
    }
}