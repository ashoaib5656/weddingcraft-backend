using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepo;

        public InventoryService(IInventoryRepository inventoryRepo)
        {
            _inventoryRepo = inventoryRepo;
        }

        public async Task<PagedResponse<IEnumerable<InventoryItem>>> GetAllAsync(PaginationFilter filter)
        {
            return await _inventoryRepo.GetQueryable().ToPagedListAsync(filter);
        }

        public async Task<InventoryItem> CreateAsync(InventoryItem item)
        {
            await _inventoryRepo.AddAsync(item);
            await _inventoryRepo.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(int id, InventoryItem item)
        {
            var existing = await _inventoryRepo.GetQueryable().FirstOrDefaultAsync(i => i.Id == id);
            if (existing != null)
            {
                existing.Name = item.Name;
                existing.Category = item.Category;
                existing.Stock = item.Stock;
                existing.Price = item.Price;
                existing.Status = item.Status;
                await _inventoryRepo.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _inventoryRepo.GetQueryable().FirstOrDefaultAsync(i => i.Id == id);
            if (item != null)
            {
                _inventoryRepo.Remove(item);
                await _inventoryRepo.SaveChangesAsync();
            }
        }
    }
}
