using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Services;
using CinemaBookingSystemDAL.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var result = await _userService.GetAllAsync(cancellationToken);
            return Ok(result);
        }


        [HttpGet("paginated")]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<UserResponseDTO>>> GetPagedUsers(int pageNumber = 1, int pageSize = 10)
        {
            var result = await _userService.GetPagedUsersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpGet("by-email/{email}")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
        {
            var user = await _userService.GetByEmailAsync(email, cancellationToken);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] UserCreateDTO dto, CancellationToken cancellationToken)
        {
            if (await _userService.ExistsByEmailAsync(dto.Email, cancellationToken))
            {
                return BadRequest("Email is already in use.");
            }

            var createdUser = await _userService.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDTO dto, CancellationToken cancellationToken)
        {
            var existingUser = await _userService.GetByIdAsync(id, cancellationToken);
            if (existingUser == null)
                return NotFound();

            var updatedUser = await _userService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedUser);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var result = await _userService.DeleteAsync(id, cancellationToken);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredUsers([FromQuery] UserFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var result = await _userService.GetFilteredUsersAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}
