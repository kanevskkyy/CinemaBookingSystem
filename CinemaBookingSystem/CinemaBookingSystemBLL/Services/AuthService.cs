using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CinemaBookingSystemBLL.DTO.Authorization;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CinemaBookingSystemBLL.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly CinemaDbContext _context;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, CinemaDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null) throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded) throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, refreshToken);

            return (accessToken, refreshToken.Token);
        }

        public async Task<(string AccessToken, string RefreshToken)> RegisterAsync(UserCreateDTO dto)
        {
            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Name
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, "Customer");

            var accessToken = await GenerateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, refreshToken);

            return (accessToken, refreshToken.Token);
        }

        public async Task<string> CreateUserByAdminAsync(UserCreateDTO dto)
        {
            if (dto.Role != "Admin" && dto.Role != "Customer")
                throw new ArgumentException("Role must be either 'Admin' or 'Customer'.");

            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Name
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, dto.Role);

            return "User created successfully";
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(TokenRefreshRequest request)
        {
            var principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null)
                throw new SecurityTokenException("Invalid token");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            var savedToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId);

            if (savedToken == null || savedToken.ExpiryDate <= DateTime.UtcNow)
                throw new SecurityTokenException("Invalid or expired refresh token");

            var user = await _userManager.FindByIdAsync(userId);
            var newAccessToken = await GenerateJwtToken(user);
            var newRefreshToken = GenerateRefreshToken();

            savedToken.Token = newRefreshToken.Token;
            savedToken.ExpiryDate = newRefreshToken.ExpiryDate;
            await _context.SaveChangesAsync();

            return (newAccessToken, newRefreshToken.Token);
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiryDate = DateTime.UtcNow.AddDays(7)
            };
        }

        private async Task SaveRefreshToken(string userId, RefreshToken refreshToken)
        {
            var existing = await _context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

            if (existing != null)
            {
                existing.Token = refreshToken.Token;
                existing.ExpiryDate = refreshToken.ExpiryDate;
            }
            else
            {
                refreshToken.UserId = userId;
                await _context.RefreshTokens.AddAsync(refreshToken);
            }

            await _context.SaveChangesAsync();
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                ValidateLifetime = false
            };

            var handler = new JwtSecurityTokenHandler();
            try
            {
                var principal = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}