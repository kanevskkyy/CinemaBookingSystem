using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Genres;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IGenreService
    {
        Task<List<GenreResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<GenreResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<GenreResponseDTO> CreateAsync(GenreCreateDTO dto, CancellationToken cancellationToken = default);
        Task<GenreResponseDTO?> UpdateAsync(Guid id, GenreUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<Dictionary<string, int>> GetMovieCountsPerGenreAsync(CancellationToken cancellationToken = default);
    }
}
