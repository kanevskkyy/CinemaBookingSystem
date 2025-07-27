using System.Security.Claims;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CinemaBookingSystemAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not allowed to perform this action."
                });

            List<UserResponseDTO> result = await userService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        [HttpGet("paged")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<PagedList<UserResponseDTO>>> GetPagedUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (!User.IsInRole("Admin"))
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not allowed to perform this action."
                });

            PagedList<UserResponseDTO> result = await userService.GetPagedUsersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId)) return Forbid();
            if (!User.IsInRole("Admin") && userId != id) return Forbid();

            UserResponseDTO? user = await userService.GetByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId)) return Forbid();

            UserResponseDTO? user = await userService.GetByIdAsync(userId, cancellationToken);
            return Ok(user);
        }

        [HttpGet("by-email/{email}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not allowed to perform this action."
                });

            UserResponseDTO? user = await userService.GetByEmailAsync(email, cancellationToken);
            return Ok(user);
        }

        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserUpdateDTO dto, CancellationToken cancellationToken)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId)) return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Invalid user ID in token."
                });

            if (!User.IsInRole("Admin") && userId != id) return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not allowed to perform this action."
                });

            UserResponseDTO updatedUser = await userService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedUser);
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto, CancellationToken cancellationToken)
        {
            string? currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(currentUserIdStr) || !Guid.TryParse(currentUserIdStr, out Guid currentUserId)) return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Cannot find or invalid your id in JWT token"
                });

            bool result = await userService.ChangePasswordAsync(currentUserId, dto, cancellationToken);
            if (!result) return BadRequest(new
            {
                message = "Incorrect current password or password requirements not met."
            });

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            string? userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId)) return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "Invalid user ID in token."
                });

            if (!User.IsInRole("Admin") && userId != id) return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    message = "You are not allowed to perform this action."
                });

            await userService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
    }
}