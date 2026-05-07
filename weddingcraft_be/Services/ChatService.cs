using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;

namespace weddingcraft_be.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatMessageRepository _chatRepo;

        public ChatService(IChatMessageRepository chatRepo)
        {
            _chatRepo = chatRepo;
        }

        public async Task<PagedResponse<IEnumerable<object>>> GetHistoryAsync(PaginationFilter filter)
        {
            var pagedResponse = await _chatRepo.GetQueryable()
                .AsNoTracking()
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => (object)new {
                    m.Id,
                    m.UserId,
                    m.UserEmail,
                    m.ConversationId,
                    m.Message,
                    m.CreatedAt
                })
                .ToPagedListAsync(filter, "Chat history retrieved successfully.");

            // The controller expects oldest-first, so we reverse it here to keep controller clean
            if (pagedResponse.Data != null)
            {
                pagedResponse.Data = pagedResponse.Data.Reverse().ToList();
            }

            return pagedResponse;
        }
    }
}
