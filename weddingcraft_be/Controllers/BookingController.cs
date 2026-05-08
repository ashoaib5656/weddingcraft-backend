using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using weddingcraft_be.Dtos;
using weddingcraft_be.Services;

namespace weddingcraft_be.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/bookings")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestBooking([FromBody] BookingRequestDto request)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var booking = await _bookingService.CreateBookingAsync(customerId, request);
            return Ok(new { message = "Booking request submitted successfully.", data = booking });
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetMyBookings()
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var bookings = await _bookingService.GetCustomerBookingsAsync(customerId);
            return Ok(new { data = bookings });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingDetails(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null) return NotFound();
            
            // Security check: Only customer or vendor of the booking can view details
            if (booking.CustomerId != userId && booking.VendorId != userId)
                return Forbid();

            return Ok(new { data = booking });
        }

        [HttpPost("{id}/payment-confirm")]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.ConfirmPaymentAsync(id, customerId);

            if (!result) return BadRequest(new { message = "Failed to confirm payment. Ensure booking is confirmed and awaiting payment." });

            return Ok(new { message = "Payment confirmed. Your booking is now secure." });
        }

        [HttpPost("{id}/cancel")]
        public async Task<IActionResult> CancelBooking(int id, [FromBody] string? notes)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _bookingService.CancelBookingAsync(id, userId, notes);

            if (!result) return BadRequest(new { message = "Failed to cancel booking. Ensure you have permission." });

            return Ok(new { message = "Booking cancelled successfully." });
        }
    }
}
