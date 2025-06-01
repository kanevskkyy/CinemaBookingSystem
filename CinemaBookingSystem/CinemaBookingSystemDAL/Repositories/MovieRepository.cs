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
    public class MovieRepository : GenericRepository<Movie, int>, IMovieRepository
    {
        public MovieRepository(CinemaDbContext context) : base(context) {
        
        }
        public async Task<List<Movie>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .Where(p => p.GenreId == genreId)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            return await dbSet
                .AsNoTracking()
                .OrderByDescending(p => p.Rating) 
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }
}
