using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingcraft_be.Models
{
    [Table("reviews", Schema = "public")]
    public class Review
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        [Column("user_email")]
        public string? UserEmail { get; set; }

        [Required]
        [Column("rating")]
        public int Rating { get; set; }

        [Required]
        [Column("comment")]
        public string Comment { get; set; } = null!;

        [Column("status")]
        public string Status { get; set; } = "Pending";

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}