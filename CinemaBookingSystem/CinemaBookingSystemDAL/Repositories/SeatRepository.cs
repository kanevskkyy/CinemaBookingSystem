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
    public class SeatRepository : GenericRepository<Seat, int>, ISeatRepository
    {
        public SeatRepository(CinemaDbContext context) : base(context) { }

        public async Task<List<Seat>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.HallId == hallId)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedList<Seat>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            return await PagedList<Seat>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
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
