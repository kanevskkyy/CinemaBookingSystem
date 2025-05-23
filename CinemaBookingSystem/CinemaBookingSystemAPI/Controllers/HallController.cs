using CinemaBookingSystemBLL.DTO.Halls;
using CinemaBookingSystemBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HallController : ControllerBase
    {
        private readonly IHallService _hallService;

        public HallController(IHallService hallService)
        {
            _hallService = hallService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<HallResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var halls = await _hallService.GetAllAsync(cancellationToken);
            return Ok(halls);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var hall = await _hallService.GetByIdAsync(id, cancellationToken);
            if (hall == null) return NotFound();
            return Ok(hall);
        }

        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
        {
            var hall = await _hallService.GetByNameAsync(name, cancellationToken);
            if (hall == null) return NotFound();
            return Ok(hall);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] HallCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var created = await _hallService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] HallUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var updated = await _hallService.UpdateAsync(id, dto, cancellationToken);
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

            var result = await _hallService.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}
