using CinemaBookingSystemBLL.DTO.Genres;
using CinemaBookingSystemBLL.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _genreService;
        public GenreController(IGenreService genreService)
        {
            _genreService = genreService;
        }
        [HttpGet]
        [ProducesResponseType(typeof(List<GenreResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            
            var genres = await _genreService.GetAllAsync(cancellationToken);
            return Ok(genres);
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(GenreResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var genre = await _genreService.GetByIdAsync(id, cancellationToken);
            if (genre == null) return NotFound();
            return Ok(genre);
        }
        [HttpPost]
        [ProducesResponseType(typeof(GenreResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] GenreCreateDTO dto, CancellationToken cancellationToken)
        {
            var created = await _genreService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] GenreUpdateDTO dto, CancellationToken cancellationToken)
        {
            var updated = await _genreService.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _genreService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();
            return NoContent();
        }
        [HttpGet("movie-counts")]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieCounts(CancellationToken cancellationToken)
        {
            var counts = await _genreService.GetMovieCountsPerGenreAsync(cancellationToken);
            return Ok(counts);
        }

        [HttpGet("exists/{name}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ExistsByName(string name, CancellationToken cancellationToken)
        {
            var exists = await _genreService.ExistsByNameAsync(name, cancellationToken);
            return Ok(exists);
        }
    }
}