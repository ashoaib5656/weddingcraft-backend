using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace weddingcraft_be.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BookingReference { get; set; } = string.Empty;

        // Relationships
        [Required]
        public Guid CustomerId { get; set; }
        [ForeignKey("CustomerId")]
        public User Customer { get; set; } = null!;

        [Required]
        public Guid VendorId { get; set; }
        [ForeignKey("VendorId")]
        public User Vendor { get; set; } = null!;

        public int? ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }

        // Event Details
        [Required]
        public DateTime EventDate { get; set; }
        
        public string EventTime { get; set; } = string.Empty;
        
        [Required]
        public string Location { get; set; } = string.Empty;

        public int GuestCount { get; set; }

        // Business Metadata
        public string Requirements { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        // Lifecycle
        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;

        [Required]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.None;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation for history
        public List<BookingStatusHistory> StatusHistory { get; set; } = new();
    }
}
