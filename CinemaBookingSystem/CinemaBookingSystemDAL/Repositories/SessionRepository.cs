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

        public async Task<Session?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderBy(p => p.Id)
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        public async Task<List<Session>> GetSessionsInHallExceptAsync(Guid hallId, Guid excludedSessionId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(s => s.HallId == hallId && s.Id != excludedSessionId)
                .Include(s => s.Movie)
                .Include(s => s.Hall)
                .OrderBy(s => s.Id)
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
