using CinemaBookingSystemBLL.DTO.Authorization;
using Microsoft.AspNetCore.Mvc;
using CinemaBookingSystemBLL.DTO.Users;
using Microsoft.AspNetCore.Authorization;
using CinemaBookingSystemBLL.Interfaces;

namespace CinemaBookingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private IAuthService service;
        public AuthController(IAuthService authService)
        {
            service = authService;
        }

        /// <summary>
        /// Authenticate user and return JWT access token (alive 15 min) and refresh token (alive 7 days).
        /// </summary>
        /// <param name="dto">User login credentials (email and password).</param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var (accessToken, refreshToken) = await service.LoginAsync(dto);

            TokenResponseDTO response = new TokenResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };

            return Ok(response);
        }

        /// <summary>
        /// Register new user and return JWT access token (alive 15 min) and refresh token (alive 7 day).
        /// </summary>
        /// <param name="dto">User registration data.</param>
        [HttpPost("register")]
        [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> Register([FromBody] UserCreateCustomerDTO dto)
        {
            var (accessToken, refreshToken) = await service.RegisterAsync(dto);
            TokenResponseDTO result = new TokenResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };
            return Ok(result);
        }

        /// <summary>
        /// Register new user with any role (Customer or Admin). Need Admin authorization
        /// </summary>
        [HttpPost("admin-create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserByAdmin([FromBody] UserCreateDTO dto)
        {
            var result = await service.CreateUserByAdminAsync(dto);
            return Ok(new
            { 
                Message = result 
            });
        }

        /// <summary>
        /// Refresh access token and return new access token and refresh token
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType(typeof(TokenResponseDTO), StatusCodes.Status200OK)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshDTO dto)
        {
            var (accessToken, refreshToken) = await service.RefreshTokenAsync(dto);
            TokenResponseDTO result = new TokenResponseDTO
            {
                Token = accessToken,
                RefreshToken = refreshToken
            };

            return Ok(result);
        }
    }
}