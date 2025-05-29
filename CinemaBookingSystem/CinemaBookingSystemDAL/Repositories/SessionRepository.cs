using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces.CinemaBookingSystemDAL.Interfaces;
using CinemaBookingSystemDAL.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class SessionRepository : GenericRepository<Session, int>, ISessionRepository
    {
        public SessionRepository(CinemaDbContext context) : base(context) { 
        
        }

        public async Task<PagedList<Session>> GetPagedSessionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = dbSet.AsQueryable();
            return await PagedList<Session>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
        }


        public async Task<List<Session>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.MovieId == movieId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Session>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.HallId == hallId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Session>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.StartTime >= start.ToUniversalTime() && p.StartTime <= end.ToUniversalTime())
                .ToListAsync(cancellationToken);
        }
    }
}
