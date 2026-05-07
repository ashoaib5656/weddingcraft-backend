using Microsoft.AspNetCore.Identity;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context, IPasswordHasher<User> hasher, IConfiguration config)
    {
        // ─── Inventory ────────────────────────────────────────────────────────
        if (!context.InventoryItems.Any())
        {
            context.InventoryItems.AddRange(
                new InventoryItem { Name = "Premium Silk Chair Covers", Category = "Decoration", Stock = 500, Price = 150, Status = "In Stock" },
                new InventoryItem { Name = "Luxury Table Runners", Category = "Decoration", Stock = 100, Price = 450, Status = "In Stock" },
                new InventoryItem { Name = "Fairy Light Curtains", Category = "Lighting", Stock = 200, Price = 1200, Status = "In Stock" },
                new InventoryItem { Name = "Exotic Flower Arrangements", Category = "Decoration", Stock = 50, Price = 5000, Status = "In Stock" },
                new InventoryItem { Name = "Professional Sound System", Category = "Audio", Stock = 10, Price = 15000, Status = "In Stock" }
            );
        }

        // ─── Tasks ────────────────────────────────────────────────────────────
        if (!context.TaskItems.Any())
        {
            context.TaskItems.AddRange(
                new TaskItem { Title = "Finalize Venue Contract", AssignedTo = "admin@weddingcraft.local", DueDate = DateTime.UtcNow.AddDays(7), Priority = "High", Status = "Pending" },
                new TaskItem { Title = "Send Invitations", AssignedTo = "staff@weddingcraft.local", DueDate = DateTime.UtcNow.AddDays(14), Priority = "Medium", Status = "Pending" }
            );
        }

        // ─── Seeded Users ────────────────────────────────────────────────────
        var adminId = SeedUser(context, hasher,
            email: config["Seed:AdminEmail"] ?? "admin@weddingcraft.local",
            password: config["Seed:AdminPassword"] ?? "Admin123!",
            role: "Admin",
            name: "Admin User");

        var managerId = SeedUser(context, hasher,
            email: config["Seed:ManagerEmail"] ?? "manager@weddingcraft.local",
            password: config["Seed:ManagerPassword"] ?? "Manager123!",
            role: "Manager",
            name: "Manager User");

        var floralId = SeedUser(context, hasher,
            email: "floral@weddingcraft.local",
            password: "Vendor123!",
            role: "Vendor",
            name: "Sarah Bloom",
            businessName: "Petals & Prose",
            category: "Decoration",
            description: "Artisanal floral designs specializing in sustainable, locally-sourced blooms for romantic garden weddings and luxury estate celebrations.",
            imageUrl: "https://images.unsplash.com/photo-1522673607200-164883e2e126?auto=format&fit=crop&w=1200&q=100",
            priceRange: "₹45,000 - ₹1,50,000");

        var coordId = SeedUser(context, hasher,
            email: "planner@weddingcraft.local",
            password: "Vendor123!",
            role: "Vendor",
            name: "Michael West",
            businessName: "West Coast Weddings",
            category: "Coordination",
            description: "Seamless luxury wedding planning and month-of coordination for sophisticated couples seeking a flawless execution and timeless aesthetic.",
            imageUrl: "https://images.unsplash.com/photo-1511795409834-ef04bbd61622?auto=format&fit=crop&w=1200&q=100",
            priceRange: "₹75,000 - ₹3,50,000");

        var photoId = SeedUser(context, hasher,
            email: "photo@weddingcraft.local",
            password: "Vendor123!",
            role: "Vendor",
            name: "Alex Rivera",
            businessName: "Lumina Cinematic",
            category: "Photography",
            description: "Visual storytelling through a cinematic lens. Capturing raw emotions and timeless elegance in every frame, specializing in grand Indian weddings.",
            imageUrl: "https://images.unsplash.com/photo-1519741497674-611481863552?auto=format&fit=crop&w=1200&q=100",
            priceRange: "₹1,20,000 - ₹5,00,000");

        var makeupId = SeedUser(context, hasher,
            email: "makeup@weddingcraft.local",
            password: "Vendor123!",
            role: "Vendor",
            name: "Elena Glass",
            businessName: "Glow Bridal Artistry",
            category: "Makeup",
            description: "Premium bridal makeup and hair styling. Redefining natural beauty with high-end techniques and luxury skincare for your special day.",
            imageUrl: "https://images.unsplash.com/photo-1487412720507-e7ab37603c6f?auto=format&fit=crop&w=1200&q=100",
            priceRange: "₹15,000 - ₹65,000");

        var inviteId = SeedUser(context, hasher,
            email: "invites@weddingcraft.local",
            password: "Vendor123!",
            role: "Vendor",
            name: "Sophie Chen",
            businessName: "Paper & Pearl",
            category: "Invitations",
            description: "Bespoke wedding stationery and luxury invitations crafted with fine Italian paper, traditional letterpress, and gold foil accents.",
            imageUrl: "https://images.unsplash.com/photo-1510076857177-74700760be15?auto=format&fit=crop&w=1200&q=100",
            priceRange: "₹25,000 - ₹1,20,000");

        var caterId = SeedUser(context, hasher,
            email: "catering@weddingcraft.local",
            password: "Vendor123!",
            role: "Vendor",
            name: "Chef David",
            businessName: "Gourmet Gala",
            category: "Catering",
            description: "Exquisite culinary experiences featuring world-class multi-cuisine menus and impeccable white-glove service for grand gala dinners.",
            imageUrl: "https://images.unsplash.com/photo-1555244162-803834f70033?auto=format&fit=crop&w=1200&q=100",
            priceRange: "₹1,200 - ₹4,500 per guest");

        var staffId = SeedUser(context, hasher,
            email: config["Seed:StaffEmail"] ?? "staff@weddingcraft.local",
            password: config["Seed:StaffPassword"] ?? "Staff123!",
            role: "Staff",
            name: "Staff Member");

        var customerId = SeedUser(context, hasher,
            email: config["Seed:CustomerEmail"] ?? "customer@weddingcraft.local",
            password: config["Seed:CustomerPassword"] ?? "Customer123!",
            role: "Customer",
            name: "John Doe",
            phone: "9999999999");

        var customer2Id = SeedUser(context, hasher,
            email: "jane@example.com",
            password: "Customer123!",
            role: "Customer",
            name: "Jane Smith",
            phone: "8888888888");

        context.SaveChanges();

        // ─── Vendor Products (Services) ──────────────────────────────────────
        if (!context.Products.Any(p => p.VendorId == photoId))
        {
            context.Products.AddRange(
                new Product { VendorId = photoId, Name = "Signature Wedding Film", Description = "Full 4K cinematic coverage with drone shots.", Price = 3500, ImageUrl = "https://images.unsplash.com/photo-1492684223066-81342ee5ff30?auto=format&fit=crop&w=800&q=80" },
                new Product { VendorId = caterId, Name = "Platinum Dinner Service", Description = "5-course tasting menu with wine pairing.", Price = 150, ImageUrl = "https://images.unsplash.com/photo-1550966841-3ee7adac166e?auto=format&fit=crop&w=800&q=80" }
            );
        }

        // ─── Orders (Bookings) ───────────────────────────────────────────────
        if (!context.Orders.Any())
        {
            var orders = new[]
            {
                new Order
                {
                    UserId = customerId,
                    VendorId = photoId,
                    Title = "Summer Wedding Celebration",
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    TotalAmount = 2500,
                    Status = "Confirmed"
                },
                new Order
                {
                    UserId = customer2Id,
                    VendorId = photoId,
                    Title = "Destination Wedding Package",
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    TotalAmount = 5000,
                    Status = "Pending"
                }
            };

            context.Orders.AddRange(orders);
            context.SaveChanges();
        }

        context.SaveChanges();
    }

    private static Guid SeedUser(
        ApplicationDbContext context,
        IPasswordHasher<User> hasher,
        string email, string password, string role,
        string? name = null,
        string? phone = null,
        string? businessName = null,
        string? category = null,
        string? description = null,
        string? imageUrl = null,
        string? priceRange = null)
    {
        var existing = context.Users.FirstOrDefault(u => u.Email == email);
        if (existing != null) return existing.Id;

        var id = Guid.NewGuid();
        var user = new User
        {
            Id = id,
            Email = email,
            Role = role,
            Name = name,
            PhoneNumber = phone
        };
        user.PasswordHash = hasher.HashPassword(user, password);
        
        if (role == "Vendor")
        {
            user.VendorProfile = new VendorProfile
            {
                UserId = id,
                BusinessName = businessName,
                Category = category,
                Description = description,
                ImageUrl = imageUrl,
                PriceRange = priceRange
            };
        }

        context.Users.Add(user);
        return id;
    }
}
