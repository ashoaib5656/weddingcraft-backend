using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class ContactMessageService : IContactMessageService
    {
        private readonly IContactMessageRepository _contactRepo;

        public ContactMessageService(IContactMessageRepository contactRepo)
        {
            _contactRepo = contactRepo;
        }

        public async Task CreateAsync(ContactMessage message)
        {
            await _contactRepo.AddAsync(message);
            await _contactRepo.SaveChangesAsync();
        }

        public async Task<PagedResponse<IEnumerable<ContactMessage>>> GetAllAsync(PaginationFilter filter)
        {
            return await _contactRepo.GetQueryable()
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .ToPagedListAsync(filter);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var message = await _contactRepo.GetQueryable().FirstOrDefaultAsync(m => m.Id == id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                await _contactRepo.SaveChangesAsync();
            }
        }
    }
}
