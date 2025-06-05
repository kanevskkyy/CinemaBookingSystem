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
        private ISeatService seatService;
        public SeatController(ISeatService seatService)
        {
            this.seatService = seatService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GettAllAsync(CancellationToken cancellationToken)
        {
            var allSeats = await seatService.GetAllAsync(cancellationToken);
            return Ok(allSeats);
        }

        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedSeats([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedSeats = await seatService.GetPagedSeatsAsync(pageNumber, pageSize);
            return Ok(pagedSeats);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var seat = await seatService.GetByIdAsync(id, cancellationToken);
            if (seat == null) return NotFound();
            return Ok(seat);
        }

        [HttpGet("by-hall/{hallId:Guid}")]
        [ProducesResponseType(typeof(List<SeatResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByHallId(Guid hallId, CancellationToken cancellationToken)
        {
            var seats = await seatService.GetByHallIdAsync(hallId, cancellationToken);
            if (seats == null) return NotFound();
            return Ok(seats);
        }

        [HttpGet("by-row-and-number/{hallId:Guid}/{rowNumber:int}/{seatNumber:int}")]
        [ProducesResponseType(typeof(SeatResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByRowAndNumber(Guid hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken)
        {
            var seat = await seatService.GetByRowAndNumberAsync(hallId, rowNumber, seatNumber, cancellationToken);
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
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var created = await seatService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] SeatUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var updated = await seatService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await seatService.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
