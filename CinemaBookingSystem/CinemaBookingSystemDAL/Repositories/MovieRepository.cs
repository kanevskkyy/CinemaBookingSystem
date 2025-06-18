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

        public IQueryable<Movie> GetAllWithGenres()
        {
            return dbSet
                .Include(p => p.MovieGenres)
                .ThenInclude(p => p.Genre)
                .OrderBy(p => p.Title);
        }


        public async Task<bool> ExistsByTitleAsync(string title, Guid? id = null, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AnyAsync(p => p.Title.ToLower() == title.ToLower() && (id == null || p.Id != id), cancellationToken);
        }

        public async Task<bool> ExistsByPosterUrlAsync(string posterUrl, Guid? id = null, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AnyAsync(p => p.PosterUrl.ToLower() == posterUrl.ToLower() && (id == null || p.Id != id), cancellationToken);
        }


        public async Task<Movie?> GetByIdWithGenresAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .OrderBy(p => p.Title)
                .Include(p => p.MovieGenres)
                .ThenInclude(p => p.Genre)
                .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
        }

        public async Task<List<Movie>> GetAllWithGenresAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .OrderBy(p => p.Title)
                .Include(m => m.MovieGenres)
                .ThenInclude(p => p.Genre)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .OrderBy(p => p.Title)
                .Include(m => m.MovieGenres)
                .ThenInclude(mg => mg.Genre)
                .OrderByDescending(p => p.Rating) 
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }
}