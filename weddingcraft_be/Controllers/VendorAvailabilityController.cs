using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using weddingcraft_be.Dtos;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VendorAvailabilityController : ControllerBase
    {
        private readonly IVendorAvailabilityService _availabilityService;

        public VendorAvailabilityController(IVendorAvailabilityService availabilityService)
        {
            _availabilityService = availabilityService;
        }

        [HttpGet("{vendorId}")]
        public async Task<IActionResult> GetAvailability(Guid vendorId)
        {
            var result = await _availabilityService.GetVendorAvailabilityAsync(vendorId);
            return Ok(new { success = true, data = result });
        }

        [Authorize(Roles = "Vendor")]
        [HttpPost("block")]
        public async Task<IActionResult> BlockDate([FromBody] BlockDateRequestDto request)
        {
            var vendorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var success = await _availabilityService.BlockDateAsync(vendorId, request);
            
            if (!success) return BadRequest(new { success = false, message = "Date is already blocked or invalid." });
            
            return Ok(new { success = true, message = "Date blocked successfully." });
        }

        [Authorize(Roles = "Vendor")]
        [HttpDelete("unblock/{id}")]
        public async Task<IActionResult> UnblockDate(int id)
        {
            var vendorId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            var success = await _availabilityService.UnblockDateAsync(id, vendorId);
            
            if (!success) return NotFound(new { success = false, message = "Availability entry not found or access denied." });
            
            return Ok(new { success = true, message = "Date unblocked successfully." });
        }
    }
}
