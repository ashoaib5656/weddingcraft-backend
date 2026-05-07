using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Dtos;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _hasher;

        public UserService(IUserRepository userRepo, IMapper mapper, IPasswordHasher<User> hasher)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _hasher = hasher;
        }

        public async Task<PagedResponse<IEnumerable<UserDto>>> GetAllAsync(PaginationFilter filter, string? role)
        {
            var query = _userRepo.GetQueryable().AsNoTracking();
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            return await query
                .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                .ToPagedListAsync(filter, "Users retrieved successfully.");
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepo.GetQueryable()
                .Include(u => u.VendorProfile)
                .Include(u => u.Products)
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(Guid id, UserDto dto)
        {
            var user = await _userRepo.GetQueryable()
                .Include(u => u.VendorProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                user.Name = dto.Name;
                user.PhoneNumber = dto.PhoneNumber;
                user.Status = dto.Status;
                user.Location = dto.Location;

                if (user.Role == "Vendor" && dto.VendorProfile != null)
                {
                    if (user.VendorProfile == null) user.VendorProfile = new VendorProfile { UserId = user.Id };
                    user.VendorProfile.Description = dto.VendorProfile.Description;
                    user.VendorProfile.ImageUrl = dto.VendorProfile.ImageUrl;
                    user.VendorProfile.PriceRange = dto.VendorProfile.PriceRange;
                    user.VendorProfile.BusinessName = dto.VendorProfile.BusinessName;
                }

                await _userRepo.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var user = await _userRepo.GetQueryable().FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                _userRepo.Remove(user);
                await _userRepo.SaveChangesAsync();
            }
        }

        public async Task UpdateProfileAsync(Guid id, UpdateProfileDto dto)
        {
            var user = await _userRepo.GetQueryable()
                .Include(u => u.VendorProfile)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                user.Name = dto.Name;
                user.Email = dto.Email;
                user.PhoneNumber = dto.PhoneNumber;
                user.Location = dto.Location;

                if (user.Role == "Vendor" && dto.VendorProfile != null)
                {
                    if (user.VendorProfile == null) user.VendorProfile = new VendorProfile { UserId = user.Id };
                    user.VendorProfile.Description = dto.VendorProfile.Description;
                    user.VendorProfile.ImageUrl = dto.VendorProfile.ImageUrl;
                    user.VendorProfile.PriceRange = dto.VendorProfile.PriceRange;
                    user.VendorProfile.BusinessName = dto.VendorProfile.BusinessName;
                }

                await _userRepo.SaveChangesAsync();
            }
        }

        public async Task ChangePasswordAsync(Guid id, string currentPassword, string newPassword)
        {
            var user = await _userRepo.GetQueryable().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new KeyNotFoundException("User not found.");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash!, currentPassword);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedAccessException("Invalid current password.");
            }

            user.PasswordHash = _hasher.HashPassword(user, newPassword);
            await _userRepo.SaveChangesAsync();
        }
    }
}
