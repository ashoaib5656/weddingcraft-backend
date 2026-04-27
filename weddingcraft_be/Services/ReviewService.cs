using Microsoft.EntityFrameworkCore;
using weddingcraft_be.Interfaces.Repositories;
using weddingcraft_be.Interfaces.Services;
using weddingcraft_be.Models;

namespace weddingcraft_be.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public async Task<IEnumerable<Review>> GetAllAsync()
        {
            return await _reviewRepo.GetQueryable()
                .Include(r => r.User)
                .AsNoTracking()
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<Review> CreateAsync(Review review)
        {
            await _reviewRepo.AddAsync(review);
            await _reviewRepo.SaveChangesAsync();
            return review;
        }

        public async Task UpdateStatusAsync(long id, string status)
        {
            var review = await _reviewRepo.GetQueryable().FirstOrDefaultAsync(r => r.Id == id);
            if (review != null)
            {
                review.Status = status;
                await _reviewRepo.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(long id)
        {
            var review = await _reviewRepo.GetQueryable().FirstOrDefaultAsync(r => r.Id == id);
            if (review != null)
            {
                _reviewRepo.Remove(review);
                await _reviewRepo.SaveChangesAsync();
            }
        }
    }
}
