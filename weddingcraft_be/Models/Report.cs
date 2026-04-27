using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class Report
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string Date { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string Author { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string Summary { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}