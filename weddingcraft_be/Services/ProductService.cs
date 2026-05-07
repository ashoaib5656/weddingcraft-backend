using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Extensions;
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

        public async Task<PagedResponse<IEnumerable<Product>>> GetAllAsync(PaginationFilter filter)
        {
            return await _productRepo.GetQueryable()
                .Include(p => p.CustomizationOptions)
                .ToPagedListAsync(filter);
        }

        public async Task<PagedResponse<IEnumerable<Product>>> GetByVendorIdAsync(Guid vendorId, PaginationFilter filter)
        {
            return await _productRepo.GetQueryable()
                .Where(p => p.VendorId == vendorId)
                .Include(p => p.CustomizationOptions)
                .ToPagedListAsync(filter);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _productRepo.GetQueryable()
                .Include(p => p.CustomizationOptions)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();
            return product;
        }

        public async Task UpdateAsync(int id, Product product)
        {
            var existing = await _productRepo.GetQueryable().FirstOrDefaultAsync(p => p.Id == id);
            if (existing != null)
            {
                existing.Name = product.Name;
                existing.Description = product.Description;
                existing.Price = product.Price;
                existing.ImageUrl = product.ImageUrl;
                await _productRepo.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _productRepo.GetQueryable().FirstOrDefaultAsync(p => p.Id == id);
            if (existing != null)
            {
                _productRepo.Remove(existing);
                await _productRepo.SaveChangesAsync();
            }
        }
    }
}
