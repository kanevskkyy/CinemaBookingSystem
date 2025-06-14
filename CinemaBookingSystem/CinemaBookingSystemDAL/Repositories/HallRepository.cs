using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus.DataSets;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class HallRepository : GenericRepository<Hall, Guid>, IHallRepository
    {
        public HallRepository(CinemaDbContext context) : base(context) { 

        }

        public async Task<bool> ExistsByNameAsync(string name, Guid? id = null, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .Where(h => h.Name.ToLower() == name.ToLower() && (id == null || h.Id != id))
                .AnyAsync(cancellationToken);
        }
    }
}
