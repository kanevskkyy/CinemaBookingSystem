using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface ISeatService
    {
        Task<List<SeatResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<SeatResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<SeatResponseDTO> CreateAsync(SeatCreateDTO dto, CancellationToken cancellationToken = default);
        Task<SeatResponseDTO?> UpdateAsync(int id, SeatUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

        Task<List<SeatResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default);
        Task<SeatResponseDTO?> GetByRowAndNumberAsync(int hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken = default);
        Task<PagedList<SeatResponseDTO>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
