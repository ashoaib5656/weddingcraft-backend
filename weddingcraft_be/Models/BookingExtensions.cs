using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingcraft_be.Models
{
    public class BookingStatusHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking Booking { get; set; } = null!;

        [Required]
        public BookingStatus OldStatus { get; set; }

        [Required]
        public BookingStatus NewStatus { get; set; }

        public string? ChangedBy { get; set; } // UserId or "System"
        
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
        
        public string? Notes { get; set; }
    }

    public class VendorAvailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid VendorId { get; set; }
        [ForeignKey("VendorId")]
        public User Vendor { get; set; } = null!;

        [Required]
        public DateTime BlockedDate { get; set; }

        public string Reason { get; set; } = "Booked";
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
