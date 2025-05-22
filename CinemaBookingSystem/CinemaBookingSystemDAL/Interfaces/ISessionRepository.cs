using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemDAL.Interfaces
{

    namespace CinemaBookingSystemDAL.Interfaces
    {
        public interface ISessionRepository : IGenericRepository<Session, int>
        {
            Task<List<Session>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
            Task<List<Session>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default);
            Task<List<Session>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
            Task<PagedList<Session>> GetPagedSessionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        }
    }
}
