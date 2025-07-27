using CinemaBookingSystemBLL.DTO.Genres;
using CinemaBookingSystemBLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private IGenreService genreService;
        public GenreController(IGenreService genreService)
        {
            this.genreService = genreService;
        }

        /// <summary>
        /// Gets all genres.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(List<GenreResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            List<GenreResponseDTO> genres = await genreService.GetAllAsync(cancellationToken);
            return Ok(genres);
        }

        /// <summary>
        /// Gets genre by ID.
        /// </summary>
        /// <param name="id">Genre ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id:Guid}")]
        [ProducesResponseType(typeof(GenreResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            GenreResponseDTO? genre = await genreService.GetByIdAsync(id, cancellationToken);
            return Ok(genre);
        }

        /// <summary>
        /// Creates a new genre (Admin only).
        /// </summary>
        /// <param name="dto">Genre create DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(GenreResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] GenreCreateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to perform this action." 
            });

            GenreResponseDTO created = await genreService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        /// <summary>
        /// Updates existing genre (Admin only).
        /// </summary>
        /// <param name="id">Genre ID.</param>
        /// <param name="dto">Genre update DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] GenreUpdateDTO dto, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to perform this action." 
            });

            await genreService.UpdateAsync(id, dto, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Deletes genre by ID (Admin only).
        /// </summary>
        /// <param name="id">Genre ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id:Guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "You are not allowed to perform this action." 
            });

            await genreService.DeleteAsync(id, cancellationToken);            
            return NoContent();
        }

        /// <summary>
        /// Gets movie counts per each genre.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("with-movie-counts")]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieCounts(CancellationToken cancellationToken)
        {
            Dictionary<string, int> counts = await genreService.GetMovieCountsPerGenreAsync(cancellationToken);
            return Ok(counts);
        }
    }
}