using CinemaBookingSystemBLL.DTO.Authorization;
using CinemaBookingSystemDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using CinemaBookingSystemBLL.DTO.Users;
using Microsoft.AspNetCore.Authorization;

namespace CinemaBookingSystemAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private UserManager<User> userManager;
        private IConfiguration configuration;
        private SignInManager<User> signInManager;

        public AuthController(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null) return Unauthorized("Invalid credentials");

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) return Unauthorized("Invalid credentials");

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserCreateDTO dto)
        {
            User user = new User
            {
                Email = dto.Email,
                UserName = dto.Name
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            await userManager.AddToRoleAsync(user, "Customer");

            string token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }

        [HttpPost("admin/create-user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUserByAdmin(UserCreateDTO dto)
        {
            string role = dto.Role?.Trim();

            if (role != "Admin" && role != "Customer") return BadRequest(new { Error = "Role must be either 'Admin' or 'Customer'." });

            User user = new User
            {
                Email = dto.Email,
                UserName = dto.Name
            };

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { Errors = errors });
            }

            await userManager.AddToRoleAsync(user, role);
            
            return Ok(new { Message = "User created successfully" });
        }

        private string GenerateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(7);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}