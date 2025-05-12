using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<UserResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> CreateAsync(UserCreateDTO dto, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
        Task<UserResponseDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<UserResponseDTO> UpdateAsync(int id, UserUpdateDTO dto, CancellationToken cancellationToken = default);
    }
}
