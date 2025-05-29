using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Interfaces;
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

        [HttpGet]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var allMovies = await movieService.GetAllAsync(cancellationToken);
            return Ok(allMovies);
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredMovies([FromQuery] MovieFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await movieService.GetFilteredMoviesAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedMovies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedMovies = await movieService.GetPagedMoviesAsync(pageNumber, pageSize);
            return Ok(pagedMovies);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MovieResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var movie = await movieService.GetByIdAsync(id, cancellationToken);
            if (movie == null) return NotFound();
            return Ok(movie);
        }

        [HttpGet("by-genre/{genreId:int}")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByGenre(int genreId, CancellationToken cancellationToken)
        {
            var movies = await movieService.GetByGenreAsync(genreId, cancellationToken);
            return Ok(movies);
        }

        [HttpGet("top-rated")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopRated(CancellationToken cancellationToken)
        {
            var movies = await movieService.GetTopRatedAsync(cancellationToken);
            return Ok(movies);
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(MovieResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MovieCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var created = await movieService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] MovieUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var updated = await movieService.UpdateAsync(id, dto, cancellationToken);
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
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await movieService.DeleteAsync(id, cancellationToken);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}