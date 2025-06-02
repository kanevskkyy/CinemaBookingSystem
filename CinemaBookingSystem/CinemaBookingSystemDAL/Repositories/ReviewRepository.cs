using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class ReviewRepository : GenericRepository<Review, int>, IReviewRepository
    {
        public ReviewRepository(CinemaDbContext context) : base(context)
        {

        }

        public async Task<List<Review>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(p => p.CreatedAt)
                .Where(r => r.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Review>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(p => p.CreatedAt)
                .Where(r => r.MovieId == movieId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Review>> GetTop10BestReviewsAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(r => r.Id)
                .Take(10)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Review>> GetTop10WorstReviewsAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(r => r.Rating)
                .ThenBy(r => r.Id)
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }
}
