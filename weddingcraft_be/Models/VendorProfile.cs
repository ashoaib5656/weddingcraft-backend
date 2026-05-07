using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingcraft_be.Models
{
    public class VendorProfile
    {
        [Key, ForeignKey("User")]
        public Guid UserId { get; set; }
        
        public User User { get; set; } = null!;

        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? PriceRange { get; set; }
        public string? BusinessName { get; set; }
        public string? Category { get; set; }
    }
}
