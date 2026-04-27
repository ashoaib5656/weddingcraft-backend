using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        public string? AssignedTo { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Pending";
    }
}