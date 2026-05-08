using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Data;
using weddingcraft_be.Dtos;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class VendorAvailabilityService : IVendorAvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public VendorAvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<VendorAvailabilityDto>> GetVendorAvailabilityAsync(Guid vendorId)
        {
            return await _context.VendorAvailabilities
                .Where(va => va.VendorId == vendorId)
                .Select(va => new VendorAvailabilityDto
                {
                    Id = va.Id,
                    VendorId = va.VendorId,
                    BlockedDate = va.BlockedDate,
                    Reason = va.Reason
                })
                .ToListAsync();
        }

        public async Task<bool> BlockDateAsync(Guid vendorId, BlockDateRequestDto request)
        {
            // Check if already blocked
            var existing = await _context.VendorAvailabilities
                .AnyAsync(va => va.VendorId == vendorId && va.BlockedDate.Date == request.Date.Date);

            if (existing) return false;

            var availability = new VendorAvailability
            {
                VendorId = vendorId,
                BlockedDate = request.Date.Date,
                Reason = request.Reason,
                CreatedAt = DateTime.UtcNow
            };

            _context.VendorAvailabilities.Add(availability);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UnblockDateAsync(int availabilityId, Guid vendorId)
        {
            var availability = await _context.VendorAvailabilities
                .FirstOrDefaultAsync(va => va.Id == availabilityId && va.VendorId == vendorId);

            if (availability == null) return false;

            _context.VendorAvailabilities.Remove(availability);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
