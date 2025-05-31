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
using Microsoft.AspNetCore.Http;
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
            List<TicketResponseDTO> result = new List<TicketResponseDTO>();

            foreach (var ticket in pagedTickets)
            {
                User user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                Session session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                Seat seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                string movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

                TicketResponseDTO tempResponseDTO = new TicketResponseDTO
                {
                    Id = ticket.Id,
                    UserId = ticket.UserId,
                    UserName = user.UserName,
                    SessionId = ticket.SessionId,
                    SessionMovieTitle = movieTitle,
                    SeatId = ticket.SeatId,
                    SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}",
                    PurchaseTime = ticket.PurchaseTime.ToUniversalTime(),
                    IsPaid = ticket.IsPaid
                };

                result.Add(tempResponseDTO);
            }

            return new PagedList<TicketResponseDTO>(result, pagedTickets.TotalCount, pagedTickets.CurrentPage, pagedTickets.PageSize);
        }

        public async Task<List<TicketResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetAllAsync(cancellationToken);
            var orderedTickets = tickets.OrderBy(m => m.Id);
            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            
            foreach (var ticket in orderedTickets)
            {
                User user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                Session session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                Seat seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                string movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
                
                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO 
                {
                    Id = ticket.Id, 
                    UserId = ticket.UserId, 
                    UserName = user.UserName, 
                    SessionId = ticket.SessionId, 
                    SessionMovieTitle = movieTitle, 
                    SeatId = ticket.SeatId, 
                    SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", 
                    PurchaseTime = ticket.PurchaseTime.ToUniversalTime(),
                    IsPaid = ticket.IsPaid
                };

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Ticket ticket = await unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return null;

            User user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
            Session session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
            Seat seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
            string movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);
            
            TicketResponseDTO ticketResponseDTO = new TicketResponseDTO 
            { 
                Id = ticket.Id, 
                UserId = ticket.UserId, 
                UserName = user.UserName, 
                SessionId = ticket.SessionId, 
                SessionMovieTitle = movieTitle, 
                SeatId = ticket.SeatId, 
                SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}", 
                PurchaseTime = ticket.PurchaseTime.ToUniversalTime(),
                IsPaid = ticket.IsPaid
            };
            return ticketResponseDTO;
        }

        public async Task<TicketResponseDTO> CreateAsync(string userId, TicketCreateDTO dto, CancellationToken cancellationToken = default)
        {
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

            Ticket createdTicket = await unitOfWork.Tickets.GetByIdAsync(ticket.Id, cancellationToken);
            if (createdTicket == null) throw new InvalidOperationException("Failed to retrieve the created ticket.");

            User user = await unitOfWork.Users.GetByIdAsync(createdTicket.UserId, cancellationToken);
            Session session = await unitOfWork.Sessions.GetByIdAsync(createdTicket.SessionId, cancellationToken);
            Seat seat = await unitOfWork.Seats.GetByIdAsync(createdTicket.SeatId, cancellationToken);
            Movie movie = await unitOfWork.Movies.GetByIdAsync(session.MovieId, cancellationToken);

            return new TicketResponseDTO
            {
                Id = createdTicket.Id,
                UserId = createdTicket.UserId,
                UserName = user.UserName,
                SessionId = createdTicket.SessionId,
                SessionMovieTitle = movie.Title,
                SeatId = createdTicket.SeatId,
                SeatInfo = $"Row {seat.RowNumber} Seat {seat.SeatNumber}",
                PurchaseTime = createdTicket.PurchaseTime,
                IsPaid = createdTicket.IsPaid
            };
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
            var orderedTickets = tickets.OrderBy(m => m.Id);

            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            foreach (var ticket in orderedTickets)
            {
                User user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                Session session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                Seat seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                string movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO
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

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        public async Task<List<TicketResponseDTO>> GetBySessionIdAsync(int sessionId, CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetBySessionIdAsync(sessionId, cancellationToken);
            var orderedTickets = tickets.OrderBy(m => m.Id);

            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            foreach (var ticket in orderedTickets)
            {
                User user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                Session session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                Seat seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                string movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO
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

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        public async Task<List<TicketResponseDTO>> GetBySeatIdAsync(int seatId, CancellationToken cancellationToken = default)
        {
            var tickets = await unitOfWork.Tickets.GetBySeatIdAsync(seatId, cancellationToken);
            var orderedTickets = tickets.OrderBy(m => m.Id);

            List<TicketResponseDTO> result = new List<TicketResponseDTO>();
            foreach (var ticket in orderedTickets)
            {
                User user = await unitOfWork.Users.GetByIdAsync(ticket.UserId, cancellationToken);
                Session session = await unitOfWork.Sessions.GetByIdAsync(ticket.SessionId, cancellationToken);
                Seat seat = await unitOfWork.Seats.GetByIdAsync(ticket.SeatId, cancellationToken);
                string movieTitle = await GetMovieTitleBySessionAsync(session, cancellationToken);

                TicketResponseDTO ticketResponseDTO = new TicketResponseDTO
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

                result.Add(ticketResponseDTO);
            }
            return result;
        }

        private async Task<string> GetMovieTitleBySessionAsync(Session? session, CancellationToken cancellationToken)
        {
            if (session == null) return "Unknown";
            Movie movie = await unitOfWork.Movies.GetByIdAsync(session.MovieId, cancellationToken);
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

            var dtoQuery = query
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
                });

            PagedList<TicketResponseDTO> pagedList = await PagedList<TicketResponseDTO>.ToPagedListAsync(dtoQuery, pageNumber, pageSize, cancellationToken);
            return pagedList;
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