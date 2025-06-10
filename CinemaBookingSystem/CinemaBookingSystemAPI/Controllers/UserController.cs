using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await userService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<UserResponseDTO>>> GetPagedUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await userService.GetPagedUsersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && userId != id) return Forbid();

            var user = await userService.GetByIdAsync(id, cancellationToken);
            if (user == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find user with this id!" });

            return Ok(user);
        }

        [HttpGet("by-email/{email}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var user = await userService.GetByEmailAsync(email, cancellationToken);
            if (user == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot find user with this email!" });

            return Ok(user);
        }

        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDTO dto, CancellationToken cancellationToken)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && userId != id) return Forbid();

            var existingUser = await userService.GetByIdAsync(id, cancellationToken);
            if (existingUser == null) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot update user with this id!" });

            var updatedUser = await userService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedUser);
        }

        [HttpPost("{id}/change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword(string id, [FromBody] ChangePasswordDTO dto, CancellationToken cancellationToken)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != id) return Forbid();

            var result = await userService.ChangePasswordAsync(id, dto, cancellationToken);
            if (!result) return BadRequest(new { message = "Incorrect current password or password requirements not met." });

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && userId != id) return Forbid();

            var result = await userService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new { message = "Cannot delete user with this id!" });

            return NoContent();
        }

        [HttpGet("filtered")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredUsers([FromQuery] UserFilterDTO filter, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new { message = "Only admins are allowed to perform this action." });

            var result = await userService.GetFilteredUsersAsync(filter, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}