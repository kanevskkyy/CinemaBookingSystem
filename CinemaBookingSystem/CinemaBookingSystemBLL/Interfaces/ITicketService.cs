using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface ITicketService
    {
        Task<List<TicketResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<TicketResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TicketResponseDTO> CreateAsync(TicketCreateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<List<TicketResponseDTO>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<List<TicketResponseDTO>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default);
        Task<List<TicketResponseDTO>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default);
        Task<PagedList<TicketResponseDTO>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }

}
