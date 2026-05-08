using System.ComponentModel.DataAnnotations;

namespace weddingcraft_be.Dtos
{
    public class VendorAvailabilityDto
    {
        public int Id { get; set; }
        public Guid VendorId { get; set; }
        public DateTime BlockedDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class BlockDateRequestDto
    {
        [Required]
        public DateTime Date { get; set; }
        public string Reason { get; set; } = "Unavailable";
    }
}
