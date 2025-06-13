using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private IMovieService movieService;

        public MovieController(IMovieService movieService)
        {
            this.movieService = movieService;
        }

        /// <summary>
        /// Get all movies.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            List<MovieResponseDTO> allMovies = await movieService.GetAllAsync(cancellationToken);
            return Ok(allMovies);
        }

        /// <summary>
        /// Get filtered movies with pagination.
        /// </summary>
        /// <param name="filter">Filter object with movie parameters.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("filtered")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredMovies([FromQuery] MovieFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            PagedList<MovieResponseDTO> result = await movieService.GetFilteredMoviesAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get paginated list of movies.
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("paginated")]
        [ProducesResponseType(typeof(PagedList<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedMovies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            PagedList<MovieResponseDTO> pagedMovies = await movieService.GetPagedMoviesAsync(pageNumber, pageSize);
            return Ok(pagedMovies);
        }

        /// <summary>
        /// Get movie by ID.
        /// </summary>
        /// <param name="id">Movie ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(MovieResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            MovieResponseDTO movie = await movieService.GetByIdAsync(id, cancellationToken);
            if (movie == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find movie with this id!" });
            
            return Ok(movie);
        }

        /// <summary>
        /// Get movies by genre ID.
        /// </summary>
        /// <param name="genreId">Genre ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-genre/{genreId:Guid}")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByGenre(Guid genreId, CancellationToken cancellationToken)
        {
            List<MovieResponseDTO> movies = await movieService.GetByGenreAsync(genreId, cancellationToken);
            return Ok(movies);
        }

        /// <summary>
        /// Get top 10 rated movies.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("top-rated")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopRated(CancellationToken cancellationToken)
        {
            List<MovieResponseDTO> movies = await movieService.GetTopRatedAsync(cancellationToken);
            return Ok(movies);
        }

        /// <summary>
        /// Create new movie (Admin only).
        /// </summary>
        /// <param name="dto">Movie create DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(MovieResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MovieCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            MovieResponseDTO created = await movieService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Update movie by ID (Admin only).
        /// </summary>
        /// <param name="id">Movie ID.</param>
        /// <param name="dto">Movie update DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] MovieUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            MovieResponseDTO updated = await movieService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot update movie with this id!" }); ;

            return NoContent();
        }

        /// <summary>
        /// Delete movie by ID (Admin only).
        /// </summary>
        /// <param name="id">Movie ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            bool result = await movieService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot delete movie with this id!" });

            return NoContent();
        }
    }
}