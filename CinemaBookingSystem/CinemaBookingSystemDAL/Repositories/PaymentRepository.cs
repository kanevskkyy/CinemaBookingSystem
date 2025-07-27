using System;
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
    public class PaymentRepository : GenericRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(CinemaDbContext context) : base(context) 
        {
            
        }

        public async Task<List<Payment>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.Ticket)
                .Where(p => p.Ticket.SessionId == sessionId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Payment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.Ticket)
                .Where(p => p.Ticket.UserId == userId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Payment>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Include(p => p.Ticket)
                .ToListAsync(cancellationToken);
        }
    }
}
