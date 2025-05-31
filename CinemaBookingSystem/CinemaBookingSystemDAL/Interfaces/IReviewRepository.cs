using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review, int>
    {
        Task<List<Review>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<Review>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
        Task<PagedList<Review>> GetPagedReviewsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<List<Review>> GetTop10BestReviewsAsync(CancellationToken cancellationToken = default);
        Task<List<Review>> GetTop10WorstReviewsAsync(CancellationToken cancellationToken = default);
    }
}
