using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Models;

namespace weddingcraft_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<CustomizationOption> CustomizationOptions => Set<CustomizationOption>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<AiRequest> AiRequests => Set<AiRequest>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<LogEntry> Logs => Set<LogEntry>();

    }
}
