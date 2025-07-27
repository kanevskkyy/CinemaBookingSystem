using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Review;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        /// <summary>
        /// Get all reviews.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            List<ReviewResponseDTO> result = await reviewService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get review by ID.
        /// </summary>
        /// <param name="id">Review ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(ReviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            ReviewResponseDTO? review = await reviewService.GetByIdAsync(id, cancellationToken);
            return Ok(review);
        }

        /// <summary>
        /// Get reviews by user ID (only for admins).
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-user/{userId}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUserId(string userId, CancellationToken cancellationToken)
        {
            List<ReviewResponseDTO> result = await reviewService.GetByUserIdAsync(userId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get reviews of the current user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my-reviews")]
        [Authorize]
        [ProducesResponseType(typeof(List<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyReviews(CancellationToken cancellationToken)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();

            List<ReviewResponseDTO> result = await reviewService.GetByUserIdAsync(currentUserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get filtered reviews for a specific movie with pagination.
        /// </summary>
        /// <param name="movieId">Movie ID.</param>
        /// <param name="filter">Filter parameters from query string.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-movie/{movieId:Guid}")]
        [ProducesResponseType(typeof(PagedList<ReviewResponseDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByMovieId(Guid movieId, [FromQuery] ReviewFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            PagedList<ReviewResponseDTO> result = await reviewService.GetFilteredReviewsAsync(movieId, filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }


        /// <summary>
        /// Create new review (only for admins or the user himself).
        /// </summary>
        /// <param name="dto">CreateReviewDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ReviewResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] ReviewCreateDTO dto, CancellationToken cancellationToken)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ReviewResponseDTO created = await reviewService.CreateAsync(dto, currentUserId, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update review by ID (only for admins or the user himself).
        /// </summary>
        /// <param name="id">Review ID.</param>
        /// <param name="dto">UpdateReviewDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(typeof(ReviewResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Update(Guid id, [FromBody] ReviewUpdateDTO dto, CancellationToken cancellationToken)
        {
            ReviewResponseDTO? existing = await reviewService.GetByIdAsync(id, cancellationToken);
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing?.UserId != currentUserId && !User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to modify this review."
            });

            ReviewResponseDTO? updated = await reviewService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updated);
        }

        /// <summary>
        /// Delete review by ID (only for admins or the user himself).
        /// </summary>
        /// <param name="id">Review ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            ReviewResponseDTO? existing = await reviewService.GetByIdAsync(id, cancellationToken);

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing?.UserId != currentUserId && !User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to modify this review."
            });

            await reviewService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}