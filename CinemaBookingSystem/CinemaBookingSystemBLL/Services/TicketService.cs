using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;
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

        public async Task<PagedList<TicketResponseDTO>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var pagedTickets = await _unitOfWork.Tickets.GetPagedTicketsAsync(pageNumber, pageSize, cancellationToken);
            var result = new List<TicketResponseDTO>();

            foreach (var ticket in pagedTickets)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                var session = await _unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                var seat = await _unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

                var dto = new TicketResponseDTO
                {
                    Id = ticket.Id,
                    UserId = ticket.UserId,
                    UserName = user.Name,
                    SessionId = ticket.SessionId,
                    SessionMovieTitle = movieTitle,
                    SeatId = ticket.SeatId,
                    SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}",
                    PurchaseTime = ticket.PurchaseTime.ToUniversalTime()
                };

                result.Add(dto);
            }

            return new PagedList<TicketResponseDTO>(result, pagedTickets.TotalCount, pagedTickets.CurrentPage, pagedTickets.PageSize);
        }

        public async Task<List<TicketResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetAllAsync(cancellationToken);
            var result = new List<TicketResponseDTO>();
            foreach (var ticket in tickets)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                var session = await _unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                var seat = await _unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO { Id = ticket.Id, UserId = ticket.UserId, UserName = user.Name, SessionId = ticket.SessionId, SessionMovieTitle = movieTitle, SeatId = ticket.SeatId, SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", PurchaseTime = ticket.PurchaseTime.ToUniversalTime() };
                result.Add(ticketResponseDTO);
            }
            return result;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var ticket = await _unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return null;

            var user = await _unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
            var session = await _unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
            var seat = await _unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
            var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
            
            TicketResponseDTO ticketResponseDTO = new TicketResponseDTO { Id = ticket.Id, UserId = ticket.UserId, UserName = user.Name, SessionId = ticket.SessionId, SessionMovieTitle = movieTitle, SeatId = ticket.SeatId, SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", PurchaseTime = ticket.PurchaseTime.ToUniversalTime() };
            return ticketResponseDTO;
        }

        public async Task<TicketResponseDTO> CreateAsync(TicketCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Ticket ticket = new Ticket { UserId = dto.UserId, SessionId = dto.SessionId, SeatId = dto.SeatId, PurchaseTime = DateTime.UtcNow.ToUniversalTime() };

            await _unitOfWork.Tickets.CreateAsync(ticket, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var t = await _unitOfWork.Tickets.GetByIdAsync(ticket.Id, cancellationToken);
            var user = await _unitOfWork.Users.GetByIdAsync(t.UserId, cancellationToken);
            var session = await _unitOfWork.Sessions.GetByIdAsync(t.SessionId, cancellationToken);
            var seat = await _unitOfWork.Seats.GetByIdAsync(t.SeatId, cancellationToken);
            var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
            TicketResponseDTO ticketResponseDTO = new TicketResponseDTO { Id = t.Id, UserId = t.UserId, UserName = user.Name, SessionId = t.SessionId, SessionMovieTitle = movieTitle, SeatId = t.SeatId, SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", PurchaseTime = t.PurchaseTime.ToUniversalTime() };

            return ticketResponseDTO;
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
            return await BuildResponseListAsync(tickets, cancellationToken);
        }

        public async Task<List<TicketResponseDTO>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetBySessionIdAsync(sessionId, cancellationToken);
            return await BuildResponseListAsync(tickets, cancellationToken);
        }

        public async Task<List<TicketResponseDTO>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default)
        {
            var tickets = await _unitOfWork.Tickets.GetBySeatIdAsync(seatId, cancellationToken);
            return await BuildResponseListAsync(tickets, cancellationToken);
        }

        private async Task<List<TicketResponseDTO>> BuildResponseListAsync(IEnumerable<Ticket> tickets, CancellationToken cancellationToken)
        {
            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            foreach (var t in tickets)
            {
                var user = await _unitOfWork.Users.GetByIdAsync(t.UserId, cancellationToken);
                var session = await _unitOfWork.Sessions.GetByIdAsync(t.SessionId, cancellationToken);
                var seat = await _unitOfWork.Seats.GetByIdAsync(t.SeatId, cancellationToken);
                var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO { Id = t.Id, UserId = t.UserId, UserName = user.Name, SessionId = t.SessionId, SessionMovieTitle = movieTitle, SeatId = t.SeatId, SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", PurchaseTime = t.PurchaseTime.ToUniversalTime() };

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        private async Task<string> GetMovieTitleBySessionAsync(Session? session, CancellationToken cancellationToken)
        {
            if (session == null) return "Unknown";
            var movie = await _unitOfWork.Movies.GetByIdAsync(session.MovieId, cancellationToken);
            return movie.Title;
        }
    }
}