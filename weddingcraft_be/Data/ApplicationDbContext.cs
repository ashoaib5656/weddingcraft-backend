using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Models;

namespace weddingcraft_be.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<VendorProfile> VendorProfiles => Set<VendorProfile>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CustomizationOption> CustomizationOptions => Set<CustomizationOption>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<AiRequest> AiRequests => Set<AiRequest>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<LogEntry> Logs => Set<LogEntry>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<ContactMessage> ContactMessages => Set<ContactMessage>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingStatusHistory> BookingStatusHistories => Set<BookingStatusHistory>();
    public DbSet<VendorAvailability> VendorAvailabilities => Set<VendorAvailability>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ─── User ─────────────────────────────────────────────────────────────
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        // ─── VendorProfile (1-to-1 with User) ──────────────────────────────────
        modelBuilder.Entity<VendorProfile>()
            .HasOne(vp => vp.User)
            .WithOne(u => u.VendorProfile)
            .HasForeignKey<VendorProfile>(vp => vp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // ─── Product (Many-to-1 with Vendor) ──────────────────────────────────
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Vendor)
            .WithMany(u => u.Products)
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.SetNull);

        // ─── Order (Many-to-1 with Vendor) ───────────────────────────────────
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Vendor)
            .WithMany()
            .HasForeignKey(o => o.VendorId)
            .OnDelete(DeleteBehavior.SetNull);

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

        // ─── Booking ──────────────────────────────────────────────────────────
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Customer)
            .WithMany()
            .HasForeignKey(b => b.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Vendor)
            .WithMany()
            .HasForeignKey(b => b.VendorId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BookingStatusHistory>()
            .HasOne(h => h.Booking)
            .WithMany(b => b.StatusHistory)
            .HasForeignKey(h => h.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VendorAvailability>()
            .HasOne(va => va.Vendor)
            .WithMany()
            .HasForeignKey(va => va.VendorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
