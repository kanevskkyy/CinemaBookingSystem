using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces.CinemaBookingSystemDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(CinemaDbContext context) : base(context) { }

        public async Task<List<Session>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.MovieId == movieId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Session>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.HallId == hallId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Session>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.StartTime >= start.ToUniversalTime() && p.StartTime <= end.ToUniversalTime())
                .ToListAsync(cancellationToken);
        }
    }
}
