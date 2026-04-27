using weddingcraft_be.Data;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Models;

namespace weddingcraft_be.Repositories
{
    public class ProductRepository : BaseRepository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db) : base(db) { }
    }

    public class OrderRepository : BaseRepository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext db) : base(db) { }
    }

    public class ReviewRepository : BaseRepository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext db) : base(db) { }
    }

    public class InventoryRepository : BaseRepository<InventoryItem>, IInventoryRepository
    {
        public InventoryRepository(ApplicationDbContext db) : base(db) { }
    }

    public class TaskRepository : BaseRepository<TaskItem>, ITaskRepository
    {
        public TaskRepository(ApplicationDbContext db) : base(db) { }
    }
}