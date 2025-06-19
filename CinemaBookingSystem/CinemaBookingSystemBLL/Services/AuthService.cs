using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Authorization;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Exceptions;
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
        private UserManager<User> userManager;
        private SignInManager<User> signInManager;
        private IConfiguration config;
        private CinemaDbContext context;
        private IMapper mapper;

        public AuthService(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, CinemaDbContext context, IMapper mapper)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            config = configuration;
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginDTO dto)
        {
            User? user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (await userManager.IsLockedOutAsync(user))
            {
                throw new UnauthorizedAccessException("Your account is temporarily locked due to multiple failed login attempts. Please try again later.");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, true);
            if (!result.Succeeded)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }
            await userManager.ResetAccessFailedCountAsync(user);

            string accessToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, refreshToken);

            return (accessToken, refreshToken.Token);
        }

        public async Task<(string AccessToken, string RefreshToken)> RegisterAsync(UserCreateCustomerDTO dto)
        {
            User? existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null) throw new EmailAlreadyExistsException(dto.Email);

            User user = mapper.Map<User>(dto);

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) throw new UserCreationFailedException();

            await userManager.AddToRoleAsync(user, "Customer");

            string accessToken = await GenerateJwtToken(user);
            RefreshToken refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, refreshToken);

            return (accessToken, refreshToken.Token);
        }


        public async Task<string> CreateUserByAdminAsync(UserCreateDTO dto)
        {
            User user = mapper.Map<User>(dto);

            User? existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null) throw new EmailAlreadyExistsException(dto.Email);

            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) throw new UserCreationFailedException(); 
            
            await userManager.AddToRoleAsync(user, dto.Role);
            return "User created successfully";
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(TokenRefreshDTO request)
        {
            ClaimsPrincipal? principal = GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal == null) throw new SecurityTokenException("Invalid token");

            string? userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            RefreshToken? savedToken = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken && rt.UserId == userId);
            if (savedToken == null || savedToken.ExpiryDate <= DateTime.UtcNow) throw new SecurityTokenException("Invalid or expired refresh token");

            User? user = await userManager.FindByIdAsync(userId);
            string newAccessToken = await GenerateJwtToken(user);
            RefreshToken newRefreshToken = GenerateRefreshToken();

            savedToken.Token = newRefreshToken.Token;
            savedToken.ExpiryDate = newRefreshToken.ExpiryDate;
            await context.SaveChangesAsync();

            return (newAccessToken, newRefreshToken.Token);
        }

        private async Task<string> GenerateJwtToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            SymmetricSecurityKey? key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            SigningCredentials? creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(config["Jwt:AccessTokenExpiryMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            RefreshToken refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiryDate = DateTime.UtcNow.AddDays(int.Parse(config["Jwt:RefreshTokenExpiryDays"]))
            };
            return refreshToken;
        }

        private async Task SaveRefreshToken(string userId, RefreshToken refreshToken)
        {
            RefreshToken? existing = await context.RefreshTokens.FirstOrDefaultAsync(rt => rt.UserId == userId);

            if (existing != null)
            {
                existing.Token = refreshToken.Token;
                existing.ExpiryDate = refreshToken.ExpiryDate;
            }
            else
            {
                refreshToken.UserId = userId;
                await context.RefreshTokens.AddAsync(refreshToken);
            }

            await context.SaveChangesAsync();
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"])),
                ValidateLifetime = false
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            try
            {
                ClaimsPrincipal? principal = handler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtToken ||!jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase)) return null;
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}