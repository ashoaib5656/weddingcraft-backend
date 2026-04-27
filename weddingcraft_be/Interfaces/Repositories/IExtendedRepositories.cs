using weddingcraft_be.Models;

namespace weddingcraft_be.Interfaces.Repositories
{
    public interface IProductRepository : IBaseRepository<Product> { }
    public interface IOrderRepository : IBaseRepository<Order> { }
    public interface IReviewRepository : IBaseRepository<Review> { }
    public interface IInventoryRepository : IBaseRepository<InventoryItem> { }
    public interface ITaskRepository : IBaseRepository<TaskItem> { }
}