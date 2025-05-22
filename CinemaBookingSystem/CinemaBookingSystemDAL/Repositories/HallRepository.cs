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
    public class HallRepository : GenericRepository<Hall, int>, IHallRepository
    {
        public HallRepository(CinemaDbContext context) : base(context) { }

        public async Task<Hall?> GetByNameAsync(string hallName, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Name == hallName, cancellationToken);
        }
    }
}
