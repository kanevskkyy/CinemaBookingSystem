using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        IQueryable<Review> GetReviewsByMovieId(Guid movieId);
        Task<bool> ExistsByUserAndMovieAsync(Guid userId, Guid movieId, CancellationToken cancellationToken = default);
        Task<List<Review>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
