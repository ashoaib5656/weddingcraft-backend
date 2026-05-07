using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly IContactMessageService _contactService;

        public ContactController(IContactMessageService contactService)
        {
            _contactService = contactService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SendMessage([FromBody] ContactMessage message)
        {
            await _contactService.CreateAsync(message);
            return Ok(new { message = "Message sent successfully" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var pagedMessages = await _contactService.GetAllAsync(filter);
            return Ok(pagedMessages);
        }

        [HttpPut("{id}/read")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _contactService.MarkAsReadAsync(id);
            return NoContent();
        }
    }
}