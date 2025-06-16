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
        public interface ISessionRepository : IGenericRepository<Session, Guid>
        {
            Task<Session?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
            IQueryable<Session> GetAllMoviesAsyncDetail(CancellationToken cancellationToken = default);
            Task<List<Session>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default);
            Task<List<Session>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default);
        }
    }
}
