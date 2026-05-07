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
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var pagedInventory = await _inventoryService.GetAllAsync(filter);
            return Ok(pagedInventory);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Create([FromBody] InventoryItem item)
        {
            var created = await _inventoryService.CreateAsync(item);
            return Ok(created);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> Update(int id, [FromBody] InventoryItem item)
        {
            await _inventoryService.UpdateAsync(id, item);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            await _inventoryService.DeleteAsync(id);
            return NoContent();
        }
    }
}