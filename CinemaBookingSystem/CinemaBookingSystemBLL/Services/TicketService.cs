using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

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
            var query = unitOfWork.Tickets.GetAllWithDetails();
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
            Ticket ticket = await unitOfWork.Tickets.GetByIdWithDetailsAsync(id, cancellationToken);
            if (ticket == null) return null;

            return mapper.Map<TicketResponseDTO>(ticket);
        }

        public async Task<TicketResponseDTO> CreateAsync(string userId, TicketCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Ticket ticket = mapper.Map<Ticket>(dto);

            await unitOfWork.Tickets.CreateAsync(ticket, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            Ticket createdTicket = await unitOfWork.Tickets.GetByIdWithDetailsAsync(ticket.Id, cancellationToken);

            if (createdTicket == null) throw new InvalidOperationException("Failed to retrieve the created ticket.");

            return mapper.Map<TicketResponseDTO>(createdTicket);
        }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Ticket ticket = await unitOfWork.Tickets.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return false;

            unitOfWork.Tickets.Delete(ticket);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<List<TicketResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            List<Ticket> tickets = await unitOfWork.Tickets.GetByUserIdAsync(userId, cancellationToken);

            return mapper.Map<List<TicketResponseDTO>>(tickets);
        }


        public async Task<List<TicketResponseDTO>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            List<Ticket> tickets = await unitOfWork.Tickets.GetBySessionIdAsync(sessionId, cancellationToken);

            return mapper.Map<List<TicketResponseDTO>>(tickets);
        }

        public async Task<List<TicketResponseDTO>> GetBySeatIdAsync(Guid seatId, CancellationToken cancellationToken = default)
        {
            List<Ticket> tickets = await unitOfWork.Tickets.GetBySeatIdAsync(seatId, cancellationToken);

            return mapper.Map<List<TicketResponseDTO>>(tickets);
        }

        public async Task<PagedList<TicketResponseDTO>> GetFilteredTicketsAsync(TicketFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Ticket> queryable = unitOfWork.Tickets.GetAll();

            if (filter.UserId.Trim().Length > 0) queryable = queryable.Where(t => t.UserId == filter.UserId);
            if (filter.SessionId.HasValue) queryable = queryable.Where(t => t.SessionId == filter.SessionId.Value);
            if (filter.SeatId.HasValue) queryable = queryable.Where(t => t.SeatId == filter.SeatId.Value);
            if (filter.PurchaseTimeFrom.HasValue) queryable = queryable.Where(t => t.PurchaseTime >= filter.PurchaseTimeFrom.Value);
            if (filter.PurchaseTimeTo.HasValue) queryable = queryable.Where(t => t.PurchaseTime <= filter.PurchaseTimeTo.Value);

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
                        if (filter.SortDescending) queryable = queryable.OrderByDescending(t => t.Id);
                        else queryable = queryable.OrderBy(t => t.Id);
                        
                        break;
                }
            }
            else queryable = queryable.OrderBy(t => t.Id);

            var projectedQuery = queryable.ProjectTo<TicketResponseDTO>(mapper.ConfigurationProvider);
            PagedList<TicketResponseDTO> pagedList = await PagedList<TicketResponseDTO>.ToPagedListAsync(projectedQuery, pageNumber, pageSize, cancellationToken);
            return pagedList;
        }
        public async Task<bool> ConfirmPaymentAsync(Guid ticketId, CancellationToken cancellationToken = default)
        {
            Ticket ticket = await unitOfWork.Tickets.GetByIdAsync(ticketId, cancellationToken);
            if (ticket == null) return false;

            ticket.IsPaid = true;
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}