using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Payment;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private ITicketService ticketService;

        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        /// <summary>
        /// Cancel reservation.
        /// </summary>
        /// <param name="ticketId">Ticket id.</param>
        [HttpPost("cancel/{ticketId}")]
        [Authorize]
        public async Task<IActionResult> CancelReservation(Guid ticketId)
        {
            string? userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                return Unauthorized("Invalid user ID in token");
            }

            await ticketService.CancelReservationAsync(ticketId, userId);

            return NoContent();
        }


        /// <summary>
        /// Get paginated list of tickets (admin only).
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("paged")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to perform this action."
            });

            PagedList<TicketResponseDTO> paginatedTickets = await ticketService.GetPagedTicketsAsync(pageNumber, pageSize);
            return Ok(paginatedTickets);
        }

        /// <summary>
        /// Get ticket by ID (admin only).
        /// </summary>
        /// <param name="id">Ticket ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new
            { 
                message = "You are not allowed to perform this action."
            });

            TicketResponseDTO? ticket = await ticketService.GetByIdAsync(id, cancellationToken);
            return Ok(ticket);
        }

        /// <summary>
        /// Create new ticket.
        /// </summary>
        /// <param name="dto">TicketCreateDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] TicketCreateDTO dto, CancellationToken cancellationToken)
        {
            string? userIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out Guid userId)) return Unauthorized();

            TicketResponseDTO created = await ticketService.CreateAsync(userId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Delete ticket by ID (admin only).
        /// </summary>
        /// <param name="id">Ticket ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new
            {
                message = "You are not allowed to perform this action."
            });

            return NoContent();
        }

        /// <summary>
        /// Get filtered tickets with pagination (admin only).
        /// </summary>
        /// <param name="filter">Filter parameters.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWithFilters([FromQuery] TicketFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            {
                message = "You are not allowed to perform this action." 
            });

            PagedList<TicketResponseDTO> result = await ticketService.GetFilteredTicketsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get current user's tickets.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTickets([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized("Invalid user ID in token");
            }

            PagedList<TicketResponseDTO> tickets = await ticketService.GetUserTicketsAsync(userId, pageNumber, pageSize);
            return Ok(tickets);
        }

        /// <summary>
        /// Confirm payment for ticket (admin only).
        /// </summary>
        /// <param name="id">Ticket ID.</param>
        /// <param name="dto"></param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{id:Guid}/confirm-payment")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ConfirmPayment(Guid id, [FromBody] PaymentConfirmDTO dto, CancellationToken cancellationToken = default)
        {
            bool result = await ticketService.ConfirmPaymentAsync(id, dto, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new 
            { 
                message = "Cannot confirm payment ticket with this id!" 
            });

            return Ok();
        }
    }
}