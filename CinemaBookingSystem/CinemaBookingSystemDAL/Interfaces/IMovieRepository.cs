using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IMovieRepository : IGenericRepository<Movie>
    {
        Task<List<Movie>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default);
        Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default);
    }
}
