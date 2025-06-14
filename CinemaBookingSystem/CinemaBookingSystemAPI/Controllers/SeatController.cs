using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private ISeatService seatService;
        public SeatController(ISeatService seatService)
        {
            this.seatService = seatService;
        }

        /// <summary>
        /// Get paginated list of seats.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSeats([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PagedList<SeatResponseDTO> pagedSeats = await seatService.GetPagedSeatsAsync(pageNumber, pageSize);
            return Ok(pagedSeats);
        }

        /// <summary>
        /// Get seat by ID.
        /// </summary>
        /// <param name="id">Seat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            SeatResponseDTO seat = await seatService.GetByIdAsync(id, cancellationToken);
            if (seat == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find seat with this id!" });
            return Ok(seat);
        }

        /// <summary>
        /// Get seats by hall ID.
        /// </summary>
        /// <param name="hallId">Hall ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-hall/{hallId:Guid}")]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByHallId(Guid hallId, CancellationToken cancellationToken)
        {
            List<SeatResponseDTO> seats = await seatService.GetByHallIdAsync(hallId, cancellationToken);
            if (seats == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find hall with this id!" });
            return Ok(seats);
        }

        /// <summary>
        /// Create new seat (admin only).
        /// </summary>
        /// <param name="dto">SeatCreateDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SeatCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            SeatResponseDTO created = await seatService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update seat by ID (admin only).
        /// </summary>
        /// <param name="id">Seat ID.</param>
        /// <param name="dto">SeatUpdateDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SeatUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            SeatResponseDTO updated = await seatService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot update seat with this id!" });
            return NoContent();
        }

        /// <summary>
        /// Delete seat by ID (admin only).
        /// </summary>
        /// <param name="id">Seat ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            bool result = await seatService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot delete seat with this id!" });
            return NoContent();
        }
    }
}