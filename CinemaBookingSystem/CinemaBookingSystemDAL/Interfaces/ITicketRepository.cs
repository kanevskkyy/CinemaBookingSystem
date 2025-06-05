using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface ITicketRepository : IGenericRepository<Ticket, Guid>
    {
        public Task<Ticket?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        public IQueryable<Ticket> GetAllWithDetails();
        Task<List<Ticket>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<List<Ticket>> GetBySeatIdAsync(Guid seatId, CancellationToken cancellationToken = default);
        
    }
}
