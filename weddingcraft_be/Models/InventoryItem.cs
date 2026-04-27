using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Models
{
    public class InventoryItem
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
        public string Category { get; set; } = null!;
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "In Stock";
    }
}