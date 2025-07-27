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
        public interface ISessionRepository : IGenericRepository<Session>
        {
            Task<List<Session>> GetSessionsInHallExceptAsync(Guid hallId, Guid excludedSessionId, CancellationToken cancellationToken = default);
            Task<Session?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
            IQueryable<Session> GetAllMoviesAsyncDetail(CancellationToken cancellationToken = default);
        }
    }
}
