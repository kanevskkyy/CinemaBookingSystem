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
        private IHallService hallService;

        public HallController(IHallService hallService)
        {
            this.hallService = hallService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<HallResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var halls = await hallService.GetAllAsync(cancellationToken);
            return Ok(halls);
        }

        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var hall = await hallService.GetByIdAsync(id, cancellationToken);
            if (hall == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find hall with this id!" });
            return Ok(hall);
        }

        [HttpGet("by-name/{name}")]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByName(string name, CancellationToken cancellationToken)
        {
            var hall = await hallService.GetByNameAsync(name, cancellationToken);
            if (hall == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find hall with this name!" });
            return Ok(hall);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] HallCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var created = await hallService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] HallUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var updated = await hallService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot update hall!" }); ;
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

            var result = await hallService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot delete hall with this id!" }); ;
            return NoContent();
        }
    }
}
