using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces;
using CinemaBookingSystemDAL.Pagination;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class MovieRepository : GenericRepository<Movie>, IMovieRepository
    {
        public MovieRepository(CinemaDbContext context) : base(context) {}
        public async Task<List<Movie>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(p => p.GenreId == genreId)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedList<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();
            return await PagedList<Movie>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
        }

        public async Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(p => p.Rating) 
                .Take(10)
                .ToListAsync(cancellationToken);
        }
    }
}
