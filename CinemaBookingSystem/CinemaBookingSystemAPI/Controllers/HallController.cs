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

        /// <summary>
        /// Get all halls.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<HallResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            List<HallResponseDTO> halls = await hallService.GetAllAsync(cancellationToken);
            return Ok(halls);
        }

        /// <summary>
        /// Get hall by ID.
        /// </summary>
        /// <param name="id">Hall ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            HallResponseDTO hall = await hallService.GetByIdAsync(id, cancellationToken);
            return Ok(hall);
        }

        /// <summary>
        /// Create new hall (Admin only).
        /// </summary>
        /// <param name="dto">Hall create DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(HallResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] HallCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            HallResponseDTO created = await hallService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update hall (Admin only).
        /// </summary>
        /// <param name="id">Hall ID.</param>
        /// <param name="dto">Hall update DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] HallUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });
            return NoContent();
        }

        /// <summary>
        /// Delete hall by ID (Admin only).
        /// </summary>
        /// <param name="id">Hall ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            bool result = await hallService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot delete hall with this id!" }); ;
            return NoContent();
        }
    }
}