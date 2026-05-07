namespace weddingcraft_be.Dtos
{
    public class VendorProfileDto
    {
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? PriceRange { get; set; }
        public string? BusinessName { get; set; }
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string Role { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public string Status { get; set; } = null!;
        public string? Location { get; set; }
        public string? Category { get; set; }
        public string? Department { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastSeen { get; set; }
        public VendorProfileDto? VendorProfile { get; set; }
    }
}