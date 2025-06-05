using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review, Guid>
    {
        Task<List<Review>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<Review>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default);
        Task<List<Review>> GetTop10BestReviewsAsync(CancellationToken cancellationToken = default);
        Task<List<Review>> GetTop10WorstReviewsAsync(CancellationToken cancellationToken = default);
    }
}
