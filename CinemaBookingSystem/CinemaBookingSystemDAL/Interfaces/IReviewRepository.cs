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
        IQueryable<Review> GetReviewsByMovieId(Guid movieId);
        Task<bool> ExistsByUserAndMovieAsync(string userId, Guid movieId, CancellationToken cancellationToken = default);
        Task<List<Review>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
