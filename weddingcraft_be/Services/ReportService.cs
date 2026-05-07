using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Common.Models;
using weddingcraft_be.Extensions;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepo;

        public ReportService(IReportRepository reportRepo)
        {
            _reportRepo = reportRepo;
        }

        public async Task<PagedResponse<IEnumerable<Report>>> GetAllAsync(PaginationFilter filter)
        {
            return await _reportRepo.GetQueryable()
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToPagedListAsync(filter);
        }

        public async Task<Report> CreateAsync(Report report)
        {
            await _reportRepo.AddAsync(report);
            await _reportRepo.SaveChangesAsync();
            return report;
        }

        public async Task DeleteAsync(int id)
        {
            var report = await _reportRepo.GetQueryable().FirstOrDefaultAsync(r => r.Id == id);
            if (report != null)
            {
                _reportRepo.Remove(report);
                await _reportRepo.SaveChangesAsync();
            }
        }
    }
}
