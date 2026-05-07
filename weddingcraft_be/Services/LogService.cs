using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class LogService : ILogService
    {
        private readonly ILogRepository _logRepo;

        public LogService(ILogRepository logRepo)
        {
            _logRepo = logRepo;
        }

        public async Task<PagedResponse<IEnumerable<object>>> GetLogsAsync(PaginationFilter filter, string? level, string? endpoint, string? userEmail)
        {
            var q = _logRepo.GetQueryable();

            if (!string.IsNullOrEmpty(level))
                q = q.Where(l => l.Level == level);

            if (!string.IsNullOrEmpty(endpoint))
                q = q.Where(l => EF.Functions.ILike(l.Endpoint ?? "", $"%{endpoint}%"));

            if (!string.IsNullOrEmpty(userEmail))
                q = q.Where(l => l.UserEmail == userEmail);

            return await q
                .OrderByDescending(l => l.Timestamp)
                .Select(l => (object)new {
                    l.Id,
                    l.Timestamp,
                    l.Level,
                    l.Message,
                    l.Exception,
                    l.UserEmail,
                    l.Endpoint,
                    l.IpAddress
                })
                .ToPagedListAsync(filter);
        }
    }
}
