using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Pagination;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserResponseDTO?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default);
        Task<UserResponseDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> UpdateAsync(string id, UserUpdateDTO dto, CancellationToken cancellationToken = default);
        Task<PagedList<UserResponseDTO>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO dto, CancellationToken cancellationToken);
    }
}
