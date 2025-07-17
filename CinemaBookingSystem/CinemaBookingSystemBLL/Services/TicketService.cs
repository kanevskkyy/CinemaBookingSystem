using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CinemaBookingSystemBLL.DTO.Payment;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.Exceptions;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CinemaBookingSystemBLL.Services
{
    public class TicketService : ITicketService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public TicketService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<PagedList<TicketResponseDTO>> GetPagedTicketsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Ticket> query = unitOfWork.Tickets.GetAllWithDetails();
            PagedList<Ticket> pagedTickets = await PagedList<Ticket>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);

            List<TicketResponseDTO> result = mapper.Map<List<TicketResponseDTO>>(pagedTickets.Items);

            return new PagedList<TicketResponseDTO>(result, pagedTickets.TotalCount, pagedTickets.CurrentPage, pagedTickets.PageSize);
        }

        public async Task<List<TicketResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<Ticket> tickets = await unitOfWork.Tickets.GetAllWithDetails().ToListAsync(cancellationToken);

            return mapper.Map<List<TicketResponseDTO>>(tickets);
        }

        public async Task<TicketResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Ticket? ticket = await unitOfWork.Tickets.GetByIdWithDetailsAsync(id, cancellationToken);
            if (ticket == null) throw new NotFoundException("Ticket", id);

            return mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> CreateAsync(string userId, TicketCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Seat? seat = await unitOfWork.Seats.GetByIdAsync(dto.SeatId, cancellationToken);
            if (seat == null) throw new NotFoundException("Seat", dto.SeatId);
                        
            Session? session = await unitOfWork.Sessions.GetByIdAsync(dto.SessionId, cancellationToken);
            if (session == null) throw new NotFoundException("Session", dto.SessionId);

            Ticket existingTicket = await unitOfWork.Tickets.GetBySeatAndSessionAsync(dto.SeatId, dto.SessionId, cancellationToken);
            if (existingTicket != null) throw new SeatAlreadyBookedException();

            if (seat.HallId != session.HallId) throw new InvalidSeatInHall();

            Ticket ticket = mapper.Map<Ticket>(dto);
            ticket.UserId = userId;

            await unitOfWork.Tickets.CreateAsync(ticket, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            Ticket? createdTicket = await unitOfWork.Tickets.GetByIdWithDetailsAsync(ticket.Id, cancellationToken);
            if (createdTicket == null) throw new InvalidOperationException("Failed to retrieve the created ticket.");

            return mapper.Map<TicketResponseDTO>(createdTicket);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Ticket? ticket = await unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) throw new NotFoundException("Ticket", id);

            unitOfWork.Tickets.Delete(ticket);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        
        private IQueryable<Ticket> ApplyFilter(IQueryable<Ticket> query, TicketFilterDTO filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.UserId)) query = query.Where(t => t.UserId == filter.UserId);

            if (filter.SessionId.HasValue) query = query.Where(t => t.SessionId == filter.SessionId.Value);

            if (filter.SeatId.HasValue) query = query.Where(t => t.SeatId == filter.SeatId.Value);

            if (filter.PurchaseTimeFrom.HasValue) query = query.Where(t => t.PurchaseTime >= filter.PurchaseTimeFrom.Value);

            if (filter.PurchaseTimeTo.HasValue) query = query.Where(t => t.PurchaseTime <= filter.PurchaseTimeTo.Value);

            return query;
        }

        private IQueryable<Ticket> ApplySorting(IQueryable<Ticket> queryable, TicketFilterDTO filter)
        {
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "purchasetime":
                        if (filter.SortDescending) queryable = queryable.OrderByDescending(t => t.PurchaseTime);
                        else queryable = queryable.OrderBy(t => t.PurchaseTime);

                        break;

                    case "userid":
                        if (filter.SortDescending) queryable = queryable.OrderByDescending(t => t.UserId);
                        else queryable = queryable.OrderBy(t => t.UserId);

                        break;

                    case "sessionid":
                        if (filter.SortDescending) queryable = queryable.OrderByDescending(t => t.SessionId);
                        else queryable = queryable.OrderBy(t => t.SessionId);

                        break;

                    case "seatid":
                        if (filter.SortDescending) queryable = queryable.OrderByDescending(t => t.SeatId);
                        else queryable = queryable.OrderBy(t => t.SeatId);

                        break;

                    default:
                        break;
                }
            }
            return queryable;
        }

        public async Task<PagedList<TicketResponseDTO>> GetFilteredTicketsAsync(TicketFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Ticket> queryable = unitOfWork.Tickets.GetAll();
            queryable = ApplyFilter(queryable, filter);
            queryable = ApplySorting(queryable, filter);
            
            var projectedQuery = queryable.ProjectTo<TicketResponseDTO>(mapper.ConfigurationProvider);
            PagedList<TicketResponseDTO> pagedList = await PagedList<TicketResponseDTO>.ToPagedListAsync(projectedQuery, pageNumber, pageSize, cancellationToken);
            return pagedList;
        }

        public async Task<bool> ConfirmPaymentAsync(Guid ticketId, PaymentConfirmDTO paymentDto, CancellationToken cancellationToken = default)
        {
            Ticket? ticket = await unitOfWork.Tickets.GetByIdAsync(ticketId, cancellationToken);
            if (ticket == null) throw new NotFoundException("Ticket", ticketId);

            if (ticket.IsPaid) throw new TicketAlreadyPaid();
            ticket.IsPaid = true;

            Payment payment = new Payment
            {
                TicketId = ticket.Id,
                Status = "Success",
                TransactionId = Guid.NewGuid().ToString(),
                PaymentMethod = paymentDto.PaymentMethod,
                PaymentDate = DateTime.UtcNow.ToUniversalTime()
            };

            await unitOfWork.Payments.CreateAsync(payment, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}