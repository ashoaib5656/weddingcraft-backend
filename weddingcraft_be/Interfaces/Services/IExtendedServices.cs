using weddingcraft_be.Models;

namespace weddingcraft_be.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
    }

    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId, string role);
        Task<Order?> GetByIdAsync(int id, Guid userId, string role);
        Task<Order> CreateAsync(Order order);
        Task UpdateStatusAsync(int id, string status);
    }

    public interface IReviewService
    {
        Task<IEnumerable<Review>> GetAllAsync();
        Task<Review> CreateAsync(Review review);
        Task UpdateStatusAsync(long id, string status);
        Task DeleteAsync(long id);
    }

    public interface IInventoryService
    {
        Task<IEnumerable<InventoryItem>> GetAllAsync();
        Task<InventoryItem> CreateAsync(InventoryItem item);
        Task UpdateAsync(int id, InventoryItem item);
        Task DeleteAsync(int id);
    }

    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllAsync();
        Task<TaskItem> CreateAsync(TaskItem task);
        Task UpdateAsync(int id, TaskItem task);
        Task DeleteAsync(int id);
    }

    public interface IAnalyticsService
    {
        Task<object> GetDashboardStatsAsync();
        Task<object> GetRevenueGrowthAsync();
    }
}