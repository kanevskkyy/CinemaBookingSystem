using CinemaBookingSystemBLL.DTO.Tickets;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Services;
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
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var pagedSessions = await _ticketService.GetAllAsync(cancellationToken);
            return Ok(pagedSessions);
        }

        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedSessions = await _ticketService.GetPagedTicketsAsync(pageNumber, pageSize);
            return Ok(pagedSessions);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var ticket = await _ticketService.GetByIdAsync(id, cancellationToken);
            if (ticket == null)
                return NotFound();

            return Ok(ticket);
        }

        [HttpGet("by-user/{userId:int}")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByUserId(int userId, CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetByUserIdAsync(userId, cancellationToken);
            if (tickets == null || !tickets.Any())
                return NotFound();

            return Ok(tickets);
        }

        [HttpGet("by-session/{sessionId:int}")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySessionId(int sessionId, CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetBySessionIdAsync(sessionId, cancellationToken);
            if (tickets == null || !tickets.Any())
                return NotFound();

            return Ok(tickets);
        }

        [HttpGet("by-seat/{seatId:int}")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBySeatId(int seatId, CancellationToken cancellationToken)
        {
            var tickets = await _ticketService.GetBySeatIdAsync(seatId, cancellationToken);
            if (tickets == null || !tickets.Any())
                return NotFound();

            return Ok(tickets);
        }

        [HttpPost]
        [ProducesResponseType(typeof(TicketResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] TicketCreateDTO dto, CancellationToken cancellationToken)
        {
            var created = await _ticketService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _ticketService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(List<TicketResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredTickets([FromQuery] TicketFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _ticketService.GetFilteredTicketsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}
