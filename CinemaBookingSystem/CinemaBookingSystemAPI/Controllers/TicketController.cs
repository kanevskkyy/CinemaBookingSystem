using System.Security.Claims;
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
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private ITicketService ticketService;

        public TicketController(ITicketService ticketService)
        {
            this.ticketService = ticketService;
        }

        /// <summary>
        /// Get all tickets (admin only).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            List<TicketResponseDTO> tickets = await ticketService.GetAllAsync(cancellationToken);
            return Ok(tickets);
        }

        /// <summary>
        /// Get paginated list of tickets (admin only).
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

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
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            TicketResponseDTO ticket = await ticketService.GetByIdAsync(id, cancellationToken);
            return Ok(ticket);
        }

        /// <summary>
        /// Get tickets by user ID (admin or own tickets).
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-user/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            string? current = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (current == null || (!User.IsInRole("Admin") && current != userId)) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied." });

            List<TicketResponseDTO> tickets = await ticketService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(tickets);
        }

        /// <summary>
        /// Get tickets by user
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyTickets(CancellationToken cancellationToken)
        {
            string? userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userID == null) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied." });

            List<TicketResponseDTO> tickets = await ticketService.GetByUserIdAsync(userID, cancellationToken);
            return Ok(tickets);
        }

        /// <summary>
        /// Get tickets by session ID (admin only).
        /// </summary>
        /// <param name="sessionId">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-session/{sessionId:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionId(Guid sessionId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            List<TicketResponseDTO> tickets = await ticketService.GetBySessionIdAsync(sessionId, cancellationToken);
            return Ok(tickets);
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
            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

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
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            bool result = await ticketService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot delete session with this id!" });

            return NoContent();
        }

        /// <summary>
        /// Get filtered tickets with pagination (admin only).
        /// </summary>
        /// <param name="filter">Filter parameters.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("filtered")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredTickets([FromQuery] TicketFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            PagedList<TicketResponseDTO> result = await ticketService.GetFilteredTicketsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Confirm payment for ticket (admin only).
        /// </summary>
        /// <param name="id">Ticket ID.</param>
        [HttpPost("{id:Guid}/confirm-payment")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            bool result = await ticketService.ConfirmPaymentAsync(id);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot confirm payment ticket with this id!" });

            return Ok();
        }
    }
}