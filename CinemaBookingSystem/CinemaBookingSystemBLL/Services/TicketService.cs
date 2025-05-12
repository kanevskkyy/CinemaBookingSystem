using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<TicketResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync(cancellationToken);
            return tickets.Select(p => new TicketResponseDTO { Id = p.Id, UserId = p.UserId, UserName = p.User.Name, SessionId = p.SessionId, SessionMovieTitle = p.Session.Movie.Title, SeatId = p.SeatId, SeatInfo = $"Row {p.Seat.RowNumber} Seat {p.Seat.SeatNumber}", PurchaseTime = p.PurchaseTime.ToUniversalTime() }).ToList();
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);

            if (ticket == null) return null;
            else return new TicketResponseDTO { Id = ticket.Id, UserId = ticket.UserId, UserName = ticket.User.Name, SessionId = ticket.SessionId, SessionMovieTitle = ticket.Session.Movie.Title, SeatId = ticket.SeatId, SeatInfo = $"Row {ticket.Seat.RowNumber} Seat {ticket.Seat.SeatNumber}", PurchaseTime = ticket.PurchaseTime.ToUniversalTime() };
        }

        public async Task<TicketResponseDTO> CreateAsync(TicketCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Ticket ticket = new Ticket { UserId = dto.UserId, SessionId = dto.SessionId, SeatId = dto.SeatId, PurchaseTime = DateTime.UtcNow.ToUniversalTime() };

            await _unitOfWork.Tickets.CreateAsync(ticket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new TicketResponseDTO { Id = ticket.Id, UserId = ticket.UserId, UserName = ticket.User.Name, SessionId = ticket.SessionId, SessionMovieTitle = ticket.Session.Movie.Title, SeatId = ticket.SeatId, SeatInfo = $"Row {ticket.Seat.RowNumber} Seat {ticket.Seat.SeatNumber}", PurchaseTime = ticket.PurchaseTime.ToUniversalTime() };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return false;

            _unitOfWork.Tickets.Delete(ticket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<TicketResponseDTO>> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetByUserIdAsync(userId, cancellationToken);
            return tickets.Select(t => new TicketResponseDTO
            { Id = t.Id, UserId = t.UserId, UserName = t.User.Name, SessionId = t.SessionId, SessionMovieTitle = t.Session.Movie.Title, SeatId = t.SeatId, SeatInfo = $"Row {t.Seat.RowNumber} Seat {t.Seat.SeatNumber}", PurchaseTime = t.PurchaseTime.ToUniversalTime() }).ToList();
        }

        public async Task<List<TicketResponseDTO>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetBySessionIdAsync(sessionId, cancellationToken);
            return tickets.Select(t => new TicketResponseDTO {Id = t.Id, UserId = t.UserId, UserName = t.User.Name, SessionId = t.SessionId, SessionMovieTitle = t.Session.Movie.Title, SeatId = t.SeatId, SeatInfo = $"Row {t.Seat.RowNumber} Seat {t.Seat.SeatNumber}", PurchaseTime = t.PurchaseTime.ToUniversalTime() }).ToList();
        }

        public async Task<List<TicketResponseDTO>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetBySeatIdAsync(seatId, cancellationToken);
            return tickets.Select(t => new TicketResponseDTO { Id = t.Id, UserId = t.UserId, UserName = t.User.Name, SessionId = t.SessionId, SessionMovieTitle = t.Session.Movie.Title, SeatId = t.SeatId, SeatInfo = $"Row {t.Seat.RowNumber} Seat {t.Seat.SeatNumber}", PurchaseTime = t.PurchaseTime.ToUniversalTime()}).ToList();
        }
    }
}
