using Microsoft.AspNetCore.Identity;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context, IPasswordHasher<User> hasher, IConfiguration config)
    {
        // ─── Products ─────────────────────────────────────────────────────────
        if (!context.Products.Any())
        {
            context.Products.AddRange(
                new Product { Name = "Wedding Card", Description = "Elegant design", Price = 200, ImageUrl = "https://placehold.co/300x200" },
                new Product { Name = "Wedding Invitation Video", Description = "HD invitation reel", Price = 500, ImageUrl = "https://placehold.co/300x200" }
            );
        }

        // ─── Inventory ────────────────────────────────────────────────────────
        if (!context.InventoryItems.Any())
        {
            context.InventoryItems.AddRange(
                new InventoryItem { Name = "Chair Covers", Category = "Decoration", Stock = 500, Price = 10, Status = "In Stock" },
                new InventoryItem { Name = "Table Runners", Category = "Decoration", Stock = 100, Price = 15, Status = "In Stock" }
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

        // ─── Seeded Users (single source of truth from config) ───────────────
        SeedUser(context, hasher,
            email: config["Seed:AdminEmail"] ?? "admin@weddingcraft.local",
            password: config["Seed:AdminPassword"] ?? "Admin123!",
            role: "Admin");

        SeedUser(context, hasher,
            email: config["Seed:ManagerEmail"] ?? "manager@weddingcraft.local",
            password: config["Seed:ManagerPassword"] ?? "Manager123!",
            role: "Manager");

        SeedUser(context, hasher,
            email: config["Seed:VendorEmail"] ?? "vendor@weddingcraft.local",
            password: config["Seed:VendorPassword"] ?? "Vendor123!",
            role: "Vendor");

        SeedUser(context, hasher,
            email: config["Seed:StaffEmail"] ?? "staff@weddingcraft.local",
            password: config["Seed:StaffPassword"] ?? "Staff123!",
            role: "Staff");

        SeedUser(context, hasher,
            email: config["Seed:CustomerEmail"] ?? "customer@weddingcraft.local",
            password: config["Seed:CustomerPassword"] ?? "Customer123!",
            role: "Customer",
            phone: "9999999999");

        context.SaveChanges();
    }

    private static void SeedUser(
        ApplicationDbContext context,
        IPasswordHasher<User> hasher,
        string email, string password, string role,
        string? phone = null)
    {
        if (context.Users.Any(u => u.Email == email)) return;

        var user = new User { Email = email, Role = role, PhoneNumber = phone };
        user.PasswordHash = hasher.HashPassword(user, password);
        context.Users.Add(user);
    }
}
