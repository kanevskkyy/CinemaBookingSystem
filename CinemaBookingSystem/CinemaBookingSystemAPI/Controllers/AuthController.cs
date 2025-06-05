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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var (accessToken, refreshToken) = await service.LoginAsync(dto);
            return Ok(new 
            { 
                Token = accessToken, 
                RefreshToken = refreshToken 
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserCreateDTO dto)
        {
            var (accessToken, refreshToken) = await service.RegisterAsync(dto);
            return Ok(new 
            { 
                Token = accessToken, 
                RefreshToken = refreshToken 
            });
        }

        [HttpPost("create")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserByAdmin([FromBody] UserCreateDTO dto)
        {
            var result = await service.CreateUserByAdminAsync(dto);
            return Ok(new
            { 
                Message = result 
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRefreshRequest dto)
        {
            var (accessToken, refreshToken) = await service.RefreshTokenAsync(dto);
            return Ok(new
            { 
                Token = accessToken, 
                RefreshToken = refreshToken 
            });
        }
    }
}