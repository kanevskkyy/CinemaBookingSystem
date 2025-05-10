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
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(CinemaDbContext context) : base(context) { }

        public async Task<User?> GetByEmailAsync(string userEmail, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Email == userEmail, cancellationToken);
        }

        public async Task<bool> ExistsByEmailAsync(string userEmail, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AnyAsync(p => p.Email == userEmail, cancellationToken);
        }

        public async Task<List<User>> GetAllWithTicketsAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Tickets)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetWithTicketsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.Id == userId, cancellationToken);
        }
    }
}
