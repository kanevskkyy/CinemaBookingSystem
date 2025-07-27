using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/seats")]
    [ApiController]
    public class SeatController : ControllerBase
    {
        private ISeatService seatService;
        public SeatController(ISeatService seatService)
        {
            this.seatService = seatService;
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
            return Ok(seats);
        }
    }
}