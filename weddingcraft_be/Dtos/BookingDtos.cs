using weddingcraft_be.Models;

namespace weddingcraft_be.Dtos
{
    public class BookingRequestDto
    {
        public Guid VendorId { get; set; }
        public int? ProductId { get; set; }
        public DateTime EventDate { get; set; }
        public string EventTime { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public string Requirements { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }

    public class BookingResponseDto
    {
        public int Id { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid VendorId { get; set; }
        public string VendorName { get; set; } = string.Empty;
        public string ServiceName { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string EventTime { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public int GuestCount { get; set; }
        public BookingStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<BookingStatusHistoryDto> StatusHistory { get; set; } = new();
    }

    public class BookingStatusHistoryDto
    {
        public BookingStatus OldStatus { get; set; }
        public BookingStatus NewStatus { get; set; }
        public string? ChangedBy { get; set; }
        public DateTime ChangedAt { get; set; }
        public string? Notes { get; set; }
    }

    public class BookingActionDto
    {
        public string? Notes { get; set; }
    }
}
