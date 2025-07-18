﻿using System.Security.Claims;
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

        /// <summary>
        /// Get all users (admin only).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "Only admins are allowed to perform this action." 
            });

            List<UserResponseDTO> result = await userService.GetAllAsync(cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Get paginated list of users (admin only).
        /// </summary>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        [HttpGet("paginated")]
        [Authorize]
        [ProducesResponseType(typeof(List<UserResponseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<PagedList<UserResponseDTO>>> GetPagedUsers(int pageNumber = 1, int pageSize = 10)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "Only admins are allowed to perform this action." 
            });

            PagedList<UserResponseDTO> result = await userService.GetPagedUsersAsync(pageNumber, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get user by ID (admin or own profile).
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && userId != id) return Forbid();

            UserResponseDTO? user = await userService.GetByIdAsync(id, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Forbid();

            UserResponseDTO? user = await userService.GetByIdAsync(userId, cancellationToken);
            return Ok(user);
        }


        /// <summary>
        /// Get user by email (admin only).
        /// </summary>
        /// <param name="email">User email.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("by-email/{email}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetByEmail(string email, CancellationToken cancellationToken)
        {
            if (!User.IsInRole("Admin")) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "Only admins are allowed to perform this action." 
            });

            UserResponseDTO? user = await userService.GetByEmailAsync(email, cancellationToken);
            return Ok(user);
        }

        /// <summary>
        /// Update user by ID (admin or own profile).
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="dto">UserUpdateDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(UserResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(string id, [FromBody] UserUpdateDTO dto, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && userId != id) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "Only admin or user who`s account is can update this account!" 
            });

            UserResponseDTO updatedUser = await userService.UpdateAsync(id, dto, cancellationToken);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Change user password.
        /// </summary>
        /// <param name="dto">ChangePasswordDTO object.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto, CancellationToken cancellationToken)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId)) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "Cannot find your id in JWT token" 
            });

            bool result = await userService.ChangePasswordAsync(currentUserId, dto, cancellationToken);
            if (!result) return BadRequest(new 
            { 
                message = "Incorrect current password or password requirements not met." 
            });

            return NoContent();
        }

        /// <summary>
        /// Delete user by ID (admin or own profile).
        /// </summary>
        /// <param name="id">User ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!User.IsInRole("Admin") && userId != id) return StatusCode(StatusCodes.Status403Forbidden, new 
            { 
                message = "Only admin or user who`s account is can delete this account!"
            });

            bool result = await userService.DeleteAsync(id, cancellationToken);
            if (!result) return StatusCode(StatusCodes.Status404NotFound, new 
            { 
                message = "Cannot delete user with this id!" 
            });

            return NoContent();
        }
    }
}