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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(CinemaDbContext context) : base(context)
        {

        }

        public IQueryable<Review> GetReviewsByMovieId(Guid movieId)
        {
            return dbSet.Where(r => r.MovieId == movieId);
        }

        public async Task<bool> ExistsByUserAndMovieAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AnyAsync(r => r.UserId == userId && r.MovieId == movieId, cancellationToken);
        }

        public async Task<List<Review>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(p => p.CreatedAt)
                .Where(r => r.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
