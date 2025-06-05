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
    public class GenreRepository : GenericRepository<Genre, Guid>, IGenreRepository
    {
        public GenreRepository(CinemaDbContext context) : base(context) 
        {

        }

        public async Task<Dictionary<string, int>> GetMovieCountsPerGenreAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Include(p => p.Movies)
                .Select(p => new { p.Name, MovieCount = p.Movies.Count })
                .OrderBy(p => p.Name)
                .ToDictionaryAsync(p => p.Name, g => g.MovieCount, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(p => p.Name == name, cancellationToken);
        }
    }
}
