using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IMovieService
    {
        Task<MovieResponseDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<List<MovieResponseDTO>> GetTopRatedAsync(CancellationToken cancellationToken = default);
        Task<MovieResponseDTO> CreateAsync(MovieCreateDTO dto, CancellationToken cancellationToken = default);
        Task<MovieResponseDTO> UpdateAsync(Guid id, MovieUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedList<MovieResponseDTO>> GetPagedMoviesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedList<MovieResponseDTO>> GetFilteredMoviesAsync(MovieFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
