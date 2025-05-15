using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedList<UserResponseDTO>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var pagedUsers = await _unitOfWork.Users.GetPagedUsersAsync(pageNumber, pageSize, cancellationToken);

            var result = pagedUsers.Select(u => new UserResponseDTO { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role }).ToList();

            return new PagedList<UserResponseDTO>(result, pagedUsers.TotalCount, pagedUsers.CurrentPage, pagedUsers.PageSize);
        }

        public async Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);
            return users.Select(u => new UserResponseDTO { Id = u.Id, Name = u.Name, Email = u.Email, Role = u.Role }).ToList();
        }

        public async Task<UserResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);

            if (user == null) return null;
            else return new UserResponseDTO{ Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role};
        }

        public async Task<UserResponseDTO> CreateAsync(UserCreateDTO dto, CancellationToken cancellationToken = default)
        {
            User user = new User { Name = dto.Name, Email = dto.Email, Password = dto.Password, Role = dto.Role };

            await _unitOfWork.Users.CreateAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserResponseDTO {Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role};
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) return false;

            _unitOfWork.Users.Delete(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<UserResponseDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email, cancellationToken);

            if (user == null) return null;
            else return new UserResponseDTO { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role};
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken);
        }
        public async Task<UserResponseDTO> UpdateAsync(int id, UserUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.Name = dto.Name;
            user.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Role))
                user.Role = dto.Role;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new UserResponseDTO { Id = user.Id, Name = user.Name, Email = user.Email, Role = user.Role };
        }
    }
}
