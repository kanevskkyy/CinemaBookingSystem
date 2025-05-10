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
        public SeatRepository(CinemaDbContext context) : base(context) { }

        public async Task<List<Seat>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.HallId == hallId)
                .ToListAsync(cancellationToken);
        }

        public async Task<Seat?> GetByRowAndNumberAsync(int hallId, int rowNumber, int columnNumber, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.HallId == hallId &&
                    p.RowNumber == rowNumber &&
                    p.SeatNumber == columnNumber,
                    cancellationToken);
        }
    }
}
