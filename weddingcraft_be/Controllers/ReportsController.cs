using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var pagedReports = await _reportService.GetAllAsync(filter);
            return Ok(pagedReports);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([FromBody] Report report)
        {
            await _reportService.CreateAsync(report);
            return Ok(report);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _reportService.DeleteAsync(id);
            return NoContent();
        }
    }
}