using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;
        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GettAllAsync(CancellationToken cancellationToken)
        {
            var allSeats = await _seatService.GetAllAsync(cancellationToken);
            return Ok(allSeats);
        }

        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSeats([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedSeats = await _seatService.GetPagedSeatsAsync(pageNumber, pageSize);
            return Ok(pagedSeats);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var seat = await _seatService.GetByIdAsync(id, cancellationToken);
            if (seat == null) return NotFound();
            return Ok(seat);
        }

        [HttpGet("by-hall/{hallId:int}")]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByHallId(int hallId, CancellationToken cancellationToken)
        {
            var seats = await _seatService.GetByHallIdAsync(hallId, cancellationToken);
            if (seats == null) return NotFound();
            return Ok(seats);
        }

        [HttpGet("by-row-and-number/{hallId:int}/{rowNumber:int}/{seatNumber:int}")]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRowAndNumber(int hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken)
        {
            var seat = await _seatService.GetByRowAndNumberAsync(hallId, rowNumber, seatNumber, cancellationToken);
            if (seat == null) return NotFound();
            return Ok(seat);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] SeatCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var created = await _seatService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] SeatUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var updated = await _seatService.UpdateAsync(id, dto, cancellationToken);
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
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await _seatService.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
