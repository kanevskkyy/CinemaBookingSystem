using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Payment;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface ITicketService
    {
        Task<PagedList<TicketResponseDTO>> GetUserTicketsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<bool> CancelReservationAsync(Guid ticketId, Guid userId, CancellationToken cancellationToken = default);
        Task<TicketResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<TicketResponseDTO> CreateAsync(Guid userId, TicketCreateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PagedList<TicketResponseDTO>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedList<TicketResponseDTO>> GetFilteredTicketsAsync(TicketFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<bool> ConfirmPaymentAsync(Guid ticketId, PaymentConfirmDTO paymentDto, CancellationToken cancellationToken = default);
    }

}
