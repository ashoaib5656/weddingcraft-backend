using weddingcraft_be.Common.Models;
using weddingcraft_be.Dtos;
using weddingcraft_be.Models;

namespace weddingcraft_be.Interfaces.Services
{
    public interface IProductService
    {
        Task<PagedResponse<IEnumerable<Product>>> GetAllAsync(PaginationFilter filter);
        Task<PagedResponse<IEnumerable<Product>>> GetByVendorIdAsync(Guid vendorId, PaginationFilter filter);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task UpdateAsync(int id, Product product);
        Task DeleteAsync(int id);
    }

    public interface IOrderService
    {
        Task<PagedResponse<IEnumerable<Order>>> GetByUserIdAsync(Guid userId, string role, PaginationFilter filter);
        Task<PagedResponse<IEnumerable<Order>>> GetByVendorIdAsync(Guid vendorId, PaginationFilter filter);
        Task<Order?> GetByIdAsync(int id, Guid userId, string role);
        Task<Order> CreateAsync(Order order);
        Task UpdateStatusAsync(int id, string status);
    }

    public interface IReviewService
    {
        Task<PagedResponse<IEnumerable<Review>>> GetAllAsync(PaginationFilter filter);
        Task<Review> CreateAsync(Review review);
        Task UpdateStatusAsync(long id, string status);
        Task DeleteAsync(long id);
    }

    public interface IInventoryService
    {
        Task<PagedResponse<IEnumerable<InventoryItem>>> GetAllAsync(PaginationFilter filter);
        Task<InventoryItem> CreateAsync(InventoryItem item);
        Task UpdateAsync(int id, InventoryItem item);
        Task DeleteAsync(int id);
    }

    public interface ITaskService
    {
        Task<PagedResponse<IEnumerable<TaskItem>>> GetAllAsync(PaginationFilter filter);
        Task<TaskItem> CreateAsync(TaskItem task);
        Task UpdateAsync(int id, TaskItem task);
        Task DeleteAsync(int id);
    }

    public interface IAnalyticsService
    {
        Task<object> GetDashboardStatsAsync();
        Task<object> GetRevenueGrowthAsync();
    }

    public interface IUserService
    {
        Task<PagedResponse<IEnumerable<UserDto>>> GetAllAsync(PaginationFilter filter, string? role);
        Task<User?> GetByIdAsync(Guid id);
        Task UpdateAsync(Guid id, UserDto dto);
        Task DeleteAsync(Guid id);
        Task UpdateProfileAsync(Guid id, UpdateProfileDto dto);
        Task ChangePasswordAsync(Guid id, string currentPassword, string newPassword);
    }



    public interface ILogService
    {
        Task<PagedResponse<IEnumerable<object>>> GetLogsAsync(PaginationFilter filter, string? level, string? endpoint, string? userEmail);
    }

    public interface IReportService
    {
        Task<PagedResponse<IEnumerable<Report>>> GetAllAsync(PaginationFilter filter);
        Task<Report> CreateAsync(Report report);
        Task DeleteAsync(int id);
    }

    public interface IContactMessageService
    {
        Task CreateAsync(ContactMessage message);
        Task<PagedResponse<IEnumerable<ContactMessage>>> GetAllAsync(PaginationFilter filter);
        Task MarkAsReadAsync(int id);
    }

    public interface IChatService
    {
        Task<PagedResponse<IEnumerable<object>>> GetHistoryAsync(PaginationFilter filter);
    }
}