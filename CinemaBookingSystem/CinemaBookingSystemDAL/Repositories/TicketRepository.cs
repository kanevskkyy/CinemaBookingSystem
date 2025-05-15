using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces;
using CinemaBookingSystemDAL.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(CinemaDbContext context) : base(context) { }

        public async Task<PagedList<Ticket>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var source = _context.Tickets.AsQueryable();
            return await PagedList<Ticket>.ToPagedListAsync(source, pageNumber, pageSize, cancellationToken);
        }


        public async Task<List<Ticket>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Ticket>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.SessionId == sessionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Ticket>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.SeatId == seatId)
                .ToListAsync(cancellationToken);
        }
    }
}
