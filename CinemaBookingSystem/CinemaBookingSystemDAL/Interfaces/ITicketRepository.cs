using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket, int>
    {
        Task<List<Ticket>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default);
        Task<PagedList<Ticket>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
