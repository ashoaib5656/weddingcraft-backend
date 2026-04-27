using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUserRepository _userRepo;
        private readonly IOrderRepository _orderRepo;

        public AnalyticsService(IUserRepository userRepo, IOrderRepository orderRepo)
        {
            _userRepo = userRepo;
            _orderRepo = orderRepo;
        }

        public async Task<object> GetDashboardStatsAsync()
        {
            var totalVendors = await _userRepo.GetQueryable().CountAsync(u => u.Role == "Vendor");
            var totalClients = await _userRepo.GetQueryable().CountAsync(u => u.Role == "Customer");
            var totalRevenue = await _orderRepo.GetQueryable().SumAsync(o => o.TotalAmount);
            var totalBookings = await _orderRepo.GetQueryable().CountAsync();

            return new
            {
                totalVendors,
                totalClients,
                totalRevenue,
                totalBookings
            };
        }

        public async Task<object> GetRevenueGrowthAsync()
        {
            // Simplified growth data for now, keeping it consistent with previous logic
            return new
            {
                categories = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun" },
                data = new[] { 60, 45, 75, 50, 90, 85 }
            };
        }
    }
}
