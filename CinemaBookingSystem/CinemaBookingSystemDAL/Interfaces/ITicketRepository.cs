using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket, int>
    {
        Task<List<Ticket>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default);
        
    }
}
