using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var pagedOrders = await _orderService.GetByUserIdAsync(userId, role, filter);
            return Ok(pagedOrders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var order = await _orderService.GetByIdAsync(id, userId, role);
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Order order)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            order.UserId = userId;
            var created = await _orderService.CreateAsync(order);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager,Vendor")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var existing = await _orderService.GetByIdAsync(id, userId, role);
            if (existing == null) return NotFound();

            await _orderService.UpdateStatusAsync(id, status);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Order status updated successfully."));
        }
    }
}