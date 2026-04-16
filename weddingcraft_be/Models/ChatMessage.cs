using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingcraft_be.Models
{
    [Table("chat_messages", Schema = "public")]
    public class ChatMessage
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("user_id")]
        public Guid? UserId { get; set; }  // nullable for anonymous

        [Column("user_email")]
        public string? UserEmail { get; set; }

        [Required]
        [Column("message")]
        public string Message { get; set; } = null!;

        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
