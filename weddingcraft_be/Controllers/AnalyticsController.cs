using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            return Ok(await _analyticsService.GetDashboardStatsAsync());
        }

        [HttpGet("revenue-growth")]
        public async Task<IActionResult> GetRevenueGrowth()
        {
            return Ok(await _analyticsService.GetRevenueGrowthAsync());
        }
    }
}