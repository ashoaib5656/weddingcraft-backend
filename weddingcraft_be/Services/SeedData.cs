using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services;

public static class SeedData
{
    public static void Initialize(ApplicationDbContext context, IPasswordHasher<User> hasher, IConfiguration config)
    {
        if (context.Products.Any()) return;

        context.Products.AddRange(
            new Product { Name = "Wedding Card", Description = "Elegant design", Price = 200, ImageUrl = "https://placehold.co/300x200" },
            new Product { Name = "Wedding Invitation Video", Description = "HD invitation reel", Price = 500, ImageUrl = "https://placehold.co/300x200" }
        );

        if (!context.Users.Any(u => u.Email == (config["Seed:AdminEmail"] ?? "")))
        {
            var adminEmail = config["Seed:AdminEmail"] ?? "admin@weddingcraft.local";
            var adminPass = config["Seed:AdminPassword"] ?? "Admin123!";
            var admin = new User { Email = adminEmail, Role = "Admin" };
            admin.PasswordHash = hasher.HashPassword(admin, adminPass);
            context.Users.Add(admin);
        }

        context.SaveChanges();
    }
}
