using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using weddingcraft_be.Dtos;
using weddingcraft_be.Services;

namespace weddingcraft_be.Controllers
{
    [Authorize(Roles = "Vendor")]
    [ApiController]
    [Route("api/vendor/bookings")]
    public class VendorBookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public VendorBookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIncomingBookings()
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var bookings = await _bookingService.GetVendorBookingsAsync(vendorId);
            return Ok(new { data = bookings });
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveBooking(int id, [FromBody] BookingActionDto action)
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.ApproveBookingAsync(id, vendorId, action.Notes);

            if (!result) return BadRequest(new { message = "Failed to approve booking. Check status and ownership." });

            return Ok(new { message = "Booking approved successfully." });
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> RejectBooking(int id, [FromBody] BookingActionDto action)
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.RejectBookingAsync(id, vendorId, action.Notes);

            if (!result) return BadRequest(new { message = "Failed to reject booking." });

            return Ok(new { message = "Booking rejected." });
        }

        [HttpPost("{id}/request-modification")]
        public async Task<IActionResult> RequestModification(int id, [FromBody] BookingActionDto action)
        {
            var vendorId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.RequestModificationAsync(id, vendorId, action.Notes);

            if (!result) return BadRequest(new { message = "Failed to request modification." });

            return Ok(new { message = "Modification request sent to customer." });
        }
    }
}
