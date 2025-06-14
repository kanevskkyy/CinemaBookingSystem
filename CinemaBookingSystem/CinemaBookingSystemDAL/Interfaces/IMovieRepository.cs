using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IMovieRepository : IGenericRepository<Movie, Guid>
    {
        Task<bool> ExistsByTitleAsync(string title, Guid? id = null, CancellationToken cancellationToken = default);
        Task<bool> ExistsByPosterUrlAsync(string posterUrl, Guid? id = null, CancellationToken cancellationToken = default);
        Task<Movie?> GetByIdWithGenresAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<Movie>> GetAllWithGenresAsync(CancellationToken cancellationToken = default);
        Task<List<Movie>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default);
        Task<List<Movie>> GetTopRatedAsync(CancellationToken cancellationToken = default);
    }
}
