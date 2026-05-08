using weddingcraft_be.Dtos;

namespace weddingcraft_be.Interfaces.Services
{
    public interface IVendorAvailabilityService
    {
        Task<List<VendorAvailabilityDto>> GetVendorAvailabilityAsync(Guid vendorId);
        Task<bool> BlockDateAsync(Guid vendorId, BlockDateRequestDto request);
        Task<bool> UnblockDateAsync(int availabilityId, Guid vendorId);
    }
}
