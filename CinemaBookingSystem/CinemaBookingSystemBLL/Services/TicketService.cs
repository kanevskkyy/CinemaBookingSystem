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
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemBLL.Services
{
    public class TicketService : ITicketService
    {
        private IUnitOfWork unitOfWork;

        public TicketService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<PagedList<TicketResponseDTO>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var pagedTickets = await unitOfWork.Tickets.GetPagedTicketsAsync(pageNumber, pageSize, cancellationToken);
            var result = new List<TicketResponseDTO>();

            foreach (var ticket in pagedTickets)
            {
                var user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                var session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                var seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

                TicketResponseDTO tempResponseDTO = new TicketResponseDTO
                {
                    Id = ticket.Id,
                    UserId = ticket.UserId,
                    UserName = user.UserName,
                    SessionId = ticket.SessionId,
                    SessionMovieTitle = movieTitle,
                    SeatId = ticket.SeatId,
                    SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}",
                    PurchaseTime = ticket.PurchaseTime.ToUniversalTime()
                };

                result.Add(tempResponseDTO);
            }

            return new PagedList<TicketResponseDTO>(result, pagedTickets.TotalCount, pagedTickets.CurrentPage, pagedTickets.PageSize);
        }

        public async Task<List<TicketResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetAllAsync(cancellationToken);
            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            
            foreach (var ticket in tickets)
            {
                var user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                
                var session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                var seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
                
                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO {
                    Id = ticket.Id, 
                    UserId = ticket.UserId, 
                    UserName = user.UserName, 
                    SessionId = ticket.SessionId, 
                    SessionMovieTitle = movieTitle, 
                    SeatId = ticket.SeatId, 
                    SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", 
                    PurchaseTime = ticket.PurchaseTime.ToUniversalTime() 
                };

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var ticket = await unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return null;

            var user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
            var session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
            var seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
            var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
            
            TicketResponseDTO ticketResponseDTO = new TicketResponseDTO { 
                Id = ticket.Id, 
                UserId = ticket.UserId, 
                UserName = user.UserName, 
                SessionId = ticket.SessionId, 
                SessionMovieTitle = movieTitle, 
                SeatId = ticket.SeatId, 
                SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", 
                PurchaseTime = ticket.PurchaseTime.ToUniversalTime() 
            };
            return ticketResponseDTO;
        }

        public async Task<TicketResponseDTO> CreateAsync(string userId, TicketCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var existingTickets = await unitOfWork.Tickets
                .FindAsync(t => t.SessionId == dto.SessionId && t.SeatId == dto.SeatId, cancellationToken);

            if (existingTickets.Any()) throw new InvalidOperationException("This seat is already taken for the selected session.");

            Ticket ticket = new Ticket
            {
                UserId = userId,
                SessionId = dto.SessionId,
                SeatId = dto.SeatId,
                PurchaseTime = DateTime.UtcNow.ToUniversalTime(),
                IsPaid = false
            };

            await unitOfWork.Tickets.CreateAsync(ticket, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var t = await unitOfWork.Tickets.GetByIdAsync(ticket.Id, cancellationToken);
            var user = await unitOfWork.Users.GetByIdAsync(t.UserId, cancellationToken);
            var session = await unitOfWork.Sessions.GetByIdAsync(t.SessionId, cancellationToken);
            var seat = await unitOfWork.Seats.GetByIdAsync(t.SeatId, cancellationToken);
            var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

            TicketResponseDTO ticketResponseDTO = new TicketResponseDTO
            {
                Id = t.Id,
                UserId = t.UserId,
                UserName = user.UserName,
                SessionId = t.SessionId,
                SessionMovieTitle = movieTitle,
                SeatId = t.SeatId,
                SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}",
                PurchaseTime = t.PurchaseTime.ToUniversalTime(),
                IsPaid = false
            };

            return ticketResponseDTO;
        }
        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var ticket = await unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return false;

            unitOfWork.Tickets.Delete(ticket);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<TicketResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetByUserIdAsync(userId, cancellationToken);
            return await BuildResponseListAsync(tickets, cancellationToken);
        }

        public async Task<List<TicketResponseDTO>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetBySessionIdAsync(sessionId, cancellationToken);
            return await BuildResponseListAsync(tickets, cancellationToken);
        }

        public async Task<List<TicketResponseDTO>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetBySeatIdAsync(seatId, cancellationToken);
            return await BuildResponseListAsync(tickets, cancellationToken);
        }

        private async Task<List<TicketResponseDTO>> BuildResponseListAsync(IEnumerable<Ticket> tickets, CancellationToken cancellationToken)
        {
            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            foreach (var ticket in tickets)
            {
                var user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                var session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                var seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                var movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
                
                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO { 
                    Id = ticket.Id, 
                    UserId = ticket.UserId, 
                    UserName = user.UserName, 
                    SessionId = ticket.SessionId, 
                    SessionMovieTitle = movieTitle, 
                    SeatId = ticket.SeatId, 
                    SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", 
                    PurchaseTime = ticket.PurchaseTime.ToUniversalTime() 
                };

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        private async Task<string> GetMovieTitleBySessionAsync(Session? session, CancellationToken cancellationToken)
        {
            if (session == null) return "Unknown";
            var movie = await unitOfWork.Movies.GetByIdAsync(session.MovieId, cancellationToken);
            return movie.Title;
        }

        public async Task<PagedList<TicketResponseDTO>> GetFilteredTicketsAsync(TicketFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Tickets.GetAll();

            if (filter.UserId.Trim().Length > 0) query = query.Where(t => t.UserId == filter.UserId);
            if (filter.SessionId.HasValue) query = query.Where(t => t.SessionId == filter.SessionId.Value);
            if (filter.SeatId.HasValue) query = query.Where(t => t.SeatId == filter.SeatId.Value);
            if (filter.PurchaseTimeFrom.HasValue) query = query.Where(t => t.PurchaseTime >= filter.PurchaseTimeFrom.Value);
            if (filter.PurchaseTimeTo.HasValue) query = query.Where(t => t.PurchaseTime <= filter.PurchaseTimeTo.Value);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "purchasetime":
                        if (filter.SortDescending) query = query.OrderByDescending(t => t.PurchaseTime);
                        else query = query.OrderBy(t => t.PurchaseTime);
                        
                        break;

                    case "userid":
                        if (filter.SortDescending) query = query.OrderByDescending(t => t.UserId);
                        else query = query.OrderBy(t => t.UserId);
                        
                        break;

                    case "sessionid":
                        if (filter.SortDescending) query = query.OrderByDescending(t => t.SessionId);
                        else query = query.OrderBy(t => t.SessionId);
                        
                        break;

                    case "seatid":
                        if (filter.SortDescending) query = query.OrderByDescending(t => t.SeatId);
                        else query = query.OrderBy(t => t.SeatId);
                        
                        break;

                    default:
                        if (filter.SortDescending) query = query.OrderByDescending(t => t.Id);
                        else query = query.OrderBy(t => t.Id);
                        
                        break;
                }
            }
            else query = query.OrderBy(t => t.Id);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TicketResponseDTO
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    SessionId = t.SessionId,
                    SeatId = t.SeatId,
                    SessionMovieTitle = t.Session.Movie.Title,
                    PurchaseTime = t.PurchaseTime,
                    UserName = t.User.UserName,
                    SeatInfo = $"Row: {t.Seat.RowNumber}, Seat: {t.Seat.SeatNumber}"
                })
                .ToListAsync(cancellationToken);

            return new PagedList<TicketResponseDTO>(items, totalCount, pageNumber, pageSize);
        }
        public async Task<bool> ConfirmPaymentAsync(int ticketId, CancellationToken cancellationToken = default)
        {
            var ticket = await unitOfWork.Tickets.GetByIdAsync(ticketId, cancellationToken);
            if (ticket == null) return false;

            ticket.IsPaid = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}