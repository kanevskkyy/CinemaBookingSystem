using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemBLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/sessions")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private ISessionService sessionService;

        public SessionController(ISessionService sessionService)
        {
            this.sessionService = sessionService;
        }

        /// <summary>
        /// Get paginated list of sessions.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSessions([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PagedList<SessionResponseDTO> pagedSessions = await sessionService.GetPagedSessionsAsync(pageNumber, pageSize);
            return Ok(pagedSessions);
        }

        /// <summary>
        /// Get session by ID.
        /// </summary>
        /// <param name="id">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(SessionResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            SessionResponseDTO? session = await sessionService.GetByIdAsync(id, cancellationToken);
            return Ok(session);
        }

        /// <summary>
        /// Create new session (admin only).
        /// </summary>
        /// <param name="dto">SessionCreateDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(SessionResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SessionCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to perform this action." 
            });

            SessionResponseDTO created = await sessionService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update session by ID (admin only).
        /// </summary>
        /// <param name="id">Session ID.</param>
        /// <param name="dto">SessionUpdateDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SessionUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to perform this action." 
            });

            SessionResponseDTO? updated = await sessionService.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Delete session by ID (admin only).
        /// </summary>
        /// <param name="id">Session ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
        /// Get filtered sessions with pagination.
        /// </summary>
        /// <param name="filter">Filter parameters.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<SessionResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWithFilteredSession([FromQuery] SessionFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            PagedList<SessionResponseDTO> result = await sessionService.GetFilteredSessionsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}