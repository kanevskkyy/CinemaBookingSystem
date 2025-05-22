using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IMovieRepository : IGenericRepository<Movie, int>
    {
        Task<List<Movie>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default);
        Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default);
        Task<PagedList<Movie>> GetPagedMoviesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
