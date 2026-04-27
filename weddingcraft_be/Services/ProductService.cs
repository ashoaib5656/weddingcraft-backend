using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;

        public ProductService(IProductRepository productRepo)
        {
            _productRepo = productRepo;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepo.GetQueryable()
                .Include(p => p.CustomizationOptions)
                .ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            // Note: If Product Id is Guid in the model, this should be updated. 
            // In Models/Product.cs it is int.
            return await _productRepo.GetQueryable()
                .Include(p => p.CustomizationOptions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
