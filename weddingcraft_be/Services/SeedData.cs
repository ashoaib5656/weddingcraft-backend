using weddingcraft_be.Data;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public static class SeedData
    {
        public static void Initialize(ApplicationDbContext context)
        {
            if (context.Products.Any()) return;

            context.Products.AddRange(
                new Product { Name = "Wedding Card", Description = "Elegant design", Price = 200, ImageUrl = "https://placehold.co/300x200" },
                new Product { Name = "Wedding Invitation Video", Description = "HD invitation reel", Price = 500, ImageUrl = "https://placehold.co/300x200" }
            );

            context.SaveChanges();
        }
    }
}
