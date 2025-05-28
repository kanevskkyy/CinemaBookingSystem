using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.DTO.Users;
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
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var pagedSessions = await _ticketService.GetAllAsync(cancellationToken);
            return Ok(pagedSessions);
        }

        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var pagedSessions = await _ticketService.GetPagedTicketsAsync(pageNumber, pageSize);
            return Ok(pagedSessions);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var ticket = await _ticketService.GetByIdAsync(id, cancellationToken);
            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        [HttpGet("by-user/{userId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var tickets = await _ticketService.GetByUserIdAsync(userId, cancellationToken);
            if (tickets == null || !tickets.Any())
                return NotFound();

            return Ok(tickets);
        }

        [HttpGet("by-session/{sessionId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionId(int sessionId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var tickets = await _ticketService.GetBySessionIdAsync(sessionId, cancellationToken);
            if (tickets == null || !tickets.Any())
                return NotFound();

            return Ok(tickets);
        }

        [HttpGet("by-seat/{seatId:int}")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySeatId(int seatId, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var tickets = await _ticketService.GetBySeatIdAsync(seatId, cancellationToken);
            if (tickets == null || !tickets.Any())
                return NotFound();

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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId)) return Unauthorized();

                var created = await _ticketService.CreateAsync(userId, dto, cancellationToken);
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

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await _ticketService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("filtered")]
        [Authorize]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredTickets([FromQuery] TicketFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await _ticketService.GetFilteredTicketsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpPost("{id}/confirm-payment")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPayment(int id)
        {
            var result = await _ticketService.ConfirmPaymentAsync(id);
            if (!result)
                return NotFound();

            return Ok();
        }
    }
}
