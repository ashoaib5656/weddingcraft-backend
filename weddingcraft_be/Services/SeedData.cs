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
