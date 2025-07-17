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
        Task<Ticket> GetBySeatAndSessionAsync(Guid seatId, Guid sessionId, CancellationToken cancellationToken = default);
        public Task<Ticket?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        public IQueryable<Ticket> GetAllWithDetails(); 
    }
}
