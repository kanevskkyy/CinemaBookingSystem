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
    public class SessionRepository : GenericRepository<Session, Guid>, ISessionRepository
    {
        public SessionRepository(CinemaDbContext context) : base(context) { 
        
        }

        public async Task<List<Session>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.MovieId == movieId)
                .OrderBy(p => p.Id)
                .Include(p => p.Movie)
                .Include(p => p.Hall)
                .ToListAsync(cancellationToken);
        }

        public async Task<Session?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<Session>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.HallId == hallId)
                .OrderBy(p => p.Id)
                .Include(p => p.Hall)
                .Include(p => p.Movie)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Session>> GetByDateRangeAsync(DateTime start, DateTime end, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.StartTime >= start.ToUniversalTime() && p.StartTime <= end.ToUniversalTime())
                .OrderBy(p => p.Id)
                .Include(p => p.Hall)
                .Include(p => p.Movie)
                .ToListAsync(cancellationToken);
        }



        public IQueryable<Session> GetAllMoviesAsyncDetail(CancellationToken cancellationToken = default)
        {
            return dbSet
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Include(p => p.Movie)
                .Include(p => p.Hall);
        }
    }
}
