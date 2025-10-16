namespace weddingcraft_be.Models
{
    public class Order
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public List<OrderItem> Items { get; set; } = new();
    }
}
