using CinemaBookingSystemBLL.DTO.Authorization;
using CinemaBookingSystemBLL.DTO.Users;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IAuthService
    {
        Task<(string AccessToken, string RefreshToken)> LoginAsync(LoginDTO dto);
        Task<(string AccessToken, string RefreshToken)> RegisterAsync(UserCreateCustomerDTO dto);
        Task<string> CreateUserByAdminAsync(UserCreateDTO dto);
        Task LogoutAsync(Guid userId);
        Task<(string AccessToken, string RefreshToken)> RefreshTokenAsync(TokenRefreshDTO request);
    }
}