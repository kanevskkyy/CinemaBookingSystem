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
            return await dbSet.AsNoTracking()
                .Select(genre => new
                {
                    genre.Name,
                    MovieCount = genre.MovieGenres.Count
                })
                .OrderBy(g => g.Name)
                .ToDictionaryAsync(g => g.Name, g => g.MovieCount, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await dbSet.AnyAsync(p => p.Name.ToLower() == name.ToLower(), cancellationToken);
        }
    }
}
