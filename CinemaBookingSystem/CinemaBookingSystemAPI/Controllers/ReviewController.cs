using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Review;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await reviewService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ReviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var review = await reviewService.GetByIdAsync(id, cancellationToken);
            if (review == null) return NotFound();
            return Ok(review);
        }

        [HttpGet("by-user/{userId}")]
        [Authorize]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && currentUserId != userId) return Forbid();

            var result = await reviewService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        [HttpGet("by-movie/{movieId}")]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByMovieId(int movieId, CancellationToken cancellationToken)
        {
            var result = await reviewService.GetByMovieIdAsync(movieId, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ReviewResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateReviewDTO dto, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (dto.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            var created = await reviewService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDTO dto, CancellationToken cancellationToken)
        {
            var existing = await reviewService.GetByIdAsync(id, cancellationToken);
            if (existing == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            var updated = await reviewService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var existing = await reviewService.GetByIdAsync(id, cancellationToken);
            if (existing == null) return NotFound();

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing.UserId != currentUserId && !User.IsInRole("Admin")) return Forbid();

            await reviewService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(PagedList<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFiltered([FromQuery] FilterReviewDto filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await reviewService.GetFilteredReviewsAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("the-best/top-10")]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTop10(CancellationToken cancellationToken = default)
        {
            var result = await reviewService.GetTop10BestReviewsAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("the-worst/top-10")]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWorst10(CancellationToken cancellationToken = default)
        {
            var result = await reviewService.GetTop10WorstReviewsAsync(cancellationToken);
            return Ok(result);
        }
    }
}