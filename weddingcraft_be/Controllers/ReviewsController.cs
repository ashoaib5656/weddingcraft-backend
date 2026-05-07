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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var pagedReviews = await _reviewService.GetAllAsync(filter);
            return Ok(pagedReviews);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Review review)
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var email = User.FindFirst(ClaimTypes.Email)!.Value;

            review.UserId = userId;
            review.UserEmail = email;
            var created = await _reviewService.CreateAsync(review);
            return Ok(created);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateStatus(long id, [FromBody] string status)
        {
            await _reviewService.UpdateStatusAsync(id, status);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(long id)
        {
            await _reviewService.DeleteAsync(id);
            return NoContent();
        }
    }
}