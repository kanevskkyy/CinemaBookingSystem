using CinemaBookingSystemBLL.DTO.Authorization;
using CinemaBookingSystemBLL.DTO.Users;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginDTO dto);
        Task<(string AccessToken, string RefreshToken)> RegisterAsync(UserCreateDTO dto);
        Task<string> CreateUserByAdminAsync(UserCreateDTO dto);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(TokenRefreshRequest request);
    }
}