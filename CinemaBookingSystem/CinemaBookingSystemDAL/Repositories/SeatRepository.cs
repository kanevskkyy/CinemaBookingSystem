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
    public class SeatRepository : GenericRepository<Seat>, ISeatRepository
    {
        public SeatRepository(CinemaDbContext context) : base(context) { 
        
        }

        public async Task<List<Seat>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(s => s.RowNumber)
                .ThenBy(s => s.SeatNumber)
                .Where(p => p.HallId == hallId)
                .ToListAsync(cancellationToken);
        }
    }
}
