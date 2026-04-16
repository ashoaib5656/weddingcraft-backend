using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Models;

namespace weddingcraft_be.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CustomizationOption> CustomizationOptions => Set<CustomizationOption>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AiRequest> AiRequests => Set<AiRequest>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LogEntry> Logs => Set<LogEntry>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── User ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // ─── RefreshToken ─────────────────────────────────────────────────────
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.Token)
            .IsUnique()
            .HasDatabaseName("IX_RefreshTokens_Token");

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(r => r.UserId)
            .HasDatabaseName("IX_RefreshTokens_UserId");

        // ─── ChatMessage ──────────────────────────────────────────────────────
        modelBuilder.Entity<ChatMessage>()
            .HasIndex(m => m.CreatedAt)
            .HasDatabaseName("IX_ChatMessages_CreatedAt");

        // ─── Log ──────────────────────────────────────────────────────────────
        modelBuilder.Entity<LogEntry>()
            .HasIndex(l => l.Timestamp)
            .HasDatabaseName("IX_Logs_Timestamp");
    }
}
