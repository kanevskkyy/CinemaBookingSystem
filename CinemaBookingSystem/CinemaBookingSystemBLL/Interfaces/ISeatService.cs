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
        Task<List<SeatResponseDTO>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default);
    }
}
