﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class TicketRepository : GenericRepository<Ticket, Guid>, ITicketRepository
    {
        public TicketRepository(CinemaDbContext context) : base(context) {
        
        }

        public async Task<Ticket> GetBySeatAndSessionAsync(Guid seatId, Guid sessionId, CancellationToken cancellationToken = default)
        {
            return await context.Tickets.FirstOrDefaultAsync(t => t.SeatId == seatId && t.SessionId == sessionId, cancellationToken);
        }

        public async Task<Ticket?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await context.Tickets
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Session)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        }

        public IQueryable<Ticket> GetAllWithDetails()
        {
            return dbSet
                .AsNoTracking()
                .Include(t => t.User)
                .Include(t => t.Session)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .OrderBy(p => p.Id);
        }

        public async Task<List<Ticket>> GetBySeatIdAsync(Guid seatId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(t => t.SeatId == seatId)
                .Include(t => t.User)
                .Include(t => t.Session)
                .ThenInclude(s => s.Movie)
                .Include(t => t.Seat)
                .OrderBy(t => t.Id)
                .ToListAsync(cancellationToken);
        }
    }
}
