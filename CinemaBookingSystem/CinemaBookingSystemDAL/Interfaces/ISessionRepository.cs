using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{

    namespace CinemaBookingSystemDAL.Interfaces
    {
        public interface ISessionRepository : IGenericRepository<Session, int>
        {
            Task<Session?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default);
            IQueryable<Session> GetAllMoviesAsyncDetail(CancellationToken cancellationToken = default);
            Task<List<Session>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
            Task<List<Session>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default);
            Task<List<Session>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        }
    }
}
