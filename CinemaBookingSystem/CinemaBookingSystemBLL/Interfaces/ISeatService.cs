using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface ISeatService
    {
        Task<SeatResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<SeatResponseDTO> CreateAsync(SeatCreateDTO dto, CancellationToken cancellationToken = default);
        Task<SeatResponseDTO?> UpdateAsync(Guid id, SeatUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

        Task<List<SeatResponseDTO>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default);
        Task<PagedList<SeatResponseDTO>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
