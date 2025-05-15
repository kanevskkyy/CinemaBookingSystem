using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPagedMovies([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var pagedMovies = await _movieService.GetPagedMoviesAsync(pageNumber, pageSize);
            return Ok(pagedMovies);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MovieResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var movie = await _movieService.GetByIdAsync(id, cancellationToken);
            if (movie == null)  return NotFound();
            return Ok(movie);
        }

        [HttpGet("by-genre/{genreId:int}")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByGenre(int genreId, CancellationToken cancellationToken)
        {
            var movies = await _movieService.GetByGenreAsync(genreId, cancellationToken);
            return Ok(movies);
        }

        [HttpGet("top-rated")]
        [ProducesResponseType(typeof(List<MovieResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopRated(CancellationToken cancellationToken)
        {
            var movies = await _movieService.GetTopRatedAsync(cancellationToken);
            return Ok(movies);
        }

        [HttpPost]
        [ProducesResponseType(typeof(MovieResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] MovieCreateDTO dto, CancellationToken cancellationToken)
        {
            var created = await _movieService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] MovieUpdateDTO dto, CancellationToken cancellationToken)
        {
            var updated = await _movieService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _movieService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}