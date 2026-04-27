using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;

        public OrderService(IOrderRepository orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, string role)
        {
            var query = _orderRepo.GetQueryable()
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .AsNoTracking();

            if (role != "Admin")
            {
                query = query.Where(o => o.UserId == userId);
            }

            return await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id, Guid userId, string role)
        {
            var order = await _orderRepo.GetQueryable()
                .Include(o => o.User)
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order != null && role != "Admin" && order.UserId != userId)
            {
                return null;
            }

            return order;
        }

        public async Task<Order> CreateAsync(Order order)
        {
            await _orderRepo.AddAsync(order);
            await _orderRepo.SaveChangesAsync();
            return order;
        }

        public async Task UpdateStatusAsync(int id, string status)
        {
            var order = await _orderRepo.GetByIdAsync(new Guid()); // Need to check if id is int or Guid
            // Based on ExtendedServices, id is int. But BaseRepository uses Guid.
            // This is a conflict in the original code. 
            // I will fix it by adding a FindIntIdAsync or similar, or updating models to Guid.
            // For now, I'll use GetQueryable to find by int ID.
            
            var existing = await _orderRepo.GetQueryable().FirstOrDefaultAsync(o => o.Id == id);
            if (existing != null)
            {
                existing.Status = status;
                await _orderRepo.SaveChangesAsync();
            }
        }
    }
}
