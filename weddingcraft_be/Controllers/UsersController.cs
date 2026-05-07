using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter, [FromQuery] string? role)
        {
            var pagedUsers = await _userService.GetAllAsync(filter, role);
            return Ok(pagedUsers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null) 
                return NotFound(ApiResponse<object>.Fail("User not found."));

            var dto = _mapper.Map<UserDto>(user);

            return Ok(ApiResponse<UserDto>.SuccessResponse(dto, "User details retrieved."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDto dto)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) 
                return NotFound(ApiResponse<object>.Fail("User not found."));

            await _userService.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.SuccessResponse(null!, "User updated successfully."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) 
                return NotFound(ApiResponse<object>.Fail("User not found."));

            await _userService.DeleteAsync(id);
            return Ok(ApiResponse<object>.SuccessResponse(null!, "User deleted successfully."));
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            
            var userId = Guid.Parse(userIdStr);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.Fail("Profile not found."));

            var dto = _mapper.Map<UserDto>(user);

            return Ok(ApiResponse<UserDto>.SuccessResponse(dto, "Profile retrieved."));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = Guid.Parse(userIdStr);
            var user = await _userService.GetByIdAsync(userId);
            if (user == null) return NotFound(ApiResponse<object>.Fail("Profile not found."));

            await _userService.UpdateProfileAsync(userId, dto);
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Profile updated successfully."));
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = Guid.Parse(userIdStr);
            try
            {
                await _userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
                return Ok(ApiResponse<object>.SuccessResponse(null!, "Password changed successfully."));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<object>.Fail("User not found."));
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid current password."));
            }
        }
    }
}