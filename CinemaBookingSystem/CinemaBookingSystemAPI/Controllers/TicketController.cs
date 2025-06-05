using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var pagedSessions = await ticketService.GetAllAsync(cancellationToken);
            return Ok(pagedSessions);
        }

        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var pagedSessions = await ticketService.GetPagedTicketsAsync(pageNumber, pageSize);
            return Ok(pagedSessions);
        }

        [HttpGet("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var ticket = await ticketService.GetByIdAsync(id, cancellationToken);
            if (ticket == null) return NotFound();

            return Ok(ticket);
        }

        [HttpGet("by-user/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            string currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (currentUserId == null || (!User.IsInRole("Admin") && currentUserId != userId)) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Access denied." });

            var tickets = await ticketService.GetByUserIdAsync(userId, cancellationToken);
            if (tickets == null || !tickets.Any()) return NotFound();

            return Ok(tickets);
        }

        [HttpGet("by-session/{sessionId:Guid}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionId(Guid sessionId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var tickets = await ticketService.GetBySessionIdAsync(sessionId, cancellationToken);
            if (tickets == null || !tickets.Any()) return NotFound();

            return Ok(tickets);
        }

        [HttpGet("by-seat/{seatId:Guid}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySeatId(Guid seatId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var tickets = await ticketService.GetBySeatIdAsync(seatId, cancellationToken);
            if (tickets == null || !tickets.Any()) return NotFound();

            return Ok(tickets);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] TicketCreateDTO dto, CancellationToken cancellationToken)
        {
            try
            {
                string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var created = await ticketService.CreateAsync(userId, dto, cancellationToken);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await ticketService.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpGet("filtered")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredTickets([FromQuery] TicketFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await ticketService.GetFilteredTicketsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id:Guid}/confirm-payment")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            var result = await ticketService.ConfirmPaymentAsync(id);
            if (!result) return NotFound();

            return Ok();
        }
    }
}
