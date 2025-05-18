using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface ISessionService
    {
        Task<List<SessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SessionResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SessionResponseDTO> CreateAsync(SessionCreateDTO dto, CancellationToken cancellationToken = default);
        Task<SessionResponseDTO?> UpdateAsync(int id, SessionUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<List<SessionResponseDTO>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default);
        Task<List<SessionResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default);
        Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
        Task<PagedList<SessionResponseDTO>> GetPagedSessionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedList<SessionResponseDTO>> GetFilteredSessionsAsync(SessionFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
