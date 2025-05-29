using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private ISessionService sessionService;

        public SessionController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var allSessions = await sessionService.GetAllAsync(cancellationToken);
            return Ok(allSessions);
        }

        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedSessions = await sessionService.GetPagedSessionsAsync(pageNumber, pageSize);
            return Ok(pagedSessions);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SessionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var session = await sessionService.GetByIdAsync(id, cancellationToken);
            if (session == null) return NotFound();

            return Ok(session);
        }

        [HttpGet("by-movie/{movieId:int}")]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByMovieId(int movieId, CancellationToken cancellationToken)
        {
            var sessions = await sessionService.GetByMovieIdAsync(movieId, cancellationToken);
            if (sessions == null || !sessions.Any()) return NotFound();

            return Ok(sessions);
        }

        [HttpGet("by-hall/{hallId:int}")]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByHallId(int hallId, CancellationToken cancellationToken)
        {
            var sessions = await sessionService.GetByHallIdAsync(hallId, cancellationToken);
            if (sessions == null || !sessions.Any()) return NotFound();

            return Ok(sessions);
        }

        [HttpGet("by-date-range")]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, CancellationToken cancellationToken)
        {
            var sessions = await sessionService.GetByDateRangeAsync(startDate, endDate, cancellationToken);
            return Ok(sessions);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SessionResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SessionCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var created = await sessionService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] SessionUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var updated = await sessionService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await sessionService.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();

            return NoContent();
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredSession([FromQuery] SessionFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await sessionService.GetFilteredSessionsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

    }
}
