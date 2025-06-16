using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface ISessionService
    {
        Task<List<SessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SessionResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SessionResponseDTO> CreateAsync(SessionCreateDTO dto, CancellationToken cancellationToken = default);
        Task<SessionResponseDTO?> UpdateAsync(Guid id, SessionUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<SessionResponseDTO>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default);
        Task<List<SessionResponseDTO>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default);
        Task<PagedList<SessionResponseDTO>> GetPagedSessionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedList<SessionResponseDTO>> GetFilteredSessionsAsync(SessionFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
