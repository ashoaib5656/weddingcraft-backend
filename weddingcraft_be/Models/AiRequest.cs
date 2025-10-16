namespace weddingcraft_be.Models
{
    public class AiRequest
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public string Prompt { get; set; } = "";
        public string Response { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
