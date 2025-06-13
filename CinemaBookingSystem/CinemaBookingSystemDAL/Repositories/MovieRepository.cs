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
    public class MovieRepository : GenericRepository<Movie, Guid>, IMovieRepository
    {
        public MovieRepository(CinemaDbContext context) : base(context) {
        
        }

        public async Task<Movie?> GetByIdWithGenresAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<List<Movie>> GetAllWithGenresAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Movie>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .Where(m => m.MovieGenres.Any(mg => mg.GenreId == genreId))
                .OrderBy(m => m.Id)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .OrderByDescending(p => p.Rating) 
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }
}