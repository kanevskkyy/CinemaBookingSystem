using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        Task<List<Seat>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default);
        Task<Seat?> GetByRowAndNumberAsync(int hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken = default);
        Task<PagedList<Seat>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
