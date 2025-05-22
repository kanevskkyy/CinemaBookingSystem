using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;
using Microsoft.EntityFrameworkCore;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.AspNetCore.Identity;

namespace CinemaBookingSystemBLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; 

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<PagedList<UserResponseDTO>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var pagedUsers = await _unitOfWork.Users.GetPagedUsersAsync(pageNumber, pageSize, cancellationToken);
            List<User> userList = pagedUsers.ToList();
            List<UserResponseDTO> result = new List<UserResponseDTO>();

            foreach (User user in userList)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.FirstOrDefault();

                result.Add(new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Role = role
                });
            }

            return new PagedList<UserResponseDTO>(result, pagedUsers.TotalCount, pagedUsers.CurrentPage, pagedUsers.PageSize);
        }

        public async Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

            List<UserResponseDTO> result = new List<UserResponseDTO>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.FirstOrDefault();

                result.Add(new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Role = role
                });
            }
            return result;
        }


        public async Task<UserResponseDTO?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) return null;

            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault() ?? "";

            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = role
            };
        }

        public async Task<UserResponseDTO> CreateAsync(UserCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var user = new User { UserName = dto.Name, Email = dto.Email };

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded) throw new Exception("Something went wrong...");

            if (!string.IsNullOrEmpty(dto.Role))
            {
                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists) throw new Exception($"Role '{dto.Role}' does not exist");

                var addRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addRoleResult.Succeeded) throw new Exception(string.Join(", ", addRoleResult.Errors.Select(e => e.Description)));
            }

            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = dto.Role
            };
        }

        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
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

            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();

            return new UserResponseDTO
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                Role = role
            };
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken);
        }
        public async Task<UserResponseDTO> UpdateAsync(string id, UserUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.UserName = dto.Name;
            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Role))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeRolesResult.Succeeded) throw new Exception("Something went wrong...");

                var roleExists = await _roleManager.RoleExistsAsync(dto.Role);
                if (!roleExists) throw new Exception($"Role '{dto.Role}' does not exist");

                var addRoleResult = await _userManager.AddToRoleAsync(user, dto.Role);
                if (!addRoleResult.Succeeded) throw new Exception("Something went wrong...");
            }

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            return new UserResponseDTO { Id = user.Id, Name = user.UserName, Email = user.Email, Role = role };
        }


        public async Task<PagedList<UserResponseDTO>> GetFilteredUsersAsync(UserFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.Users.GetAll();

            if (!string.IsNullOrWhiteSpace(filter.Name))
                query = query.Where(u => u.UserName.ToLower().Contains(filter.Name.ToLower()));
            if (!string.IsNullOrWhiteSpace(filter.Email))
                query = query.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));

           
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "name":
                        query = filter.SortDescending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName);
                        break;
                    case "email":
                        query = filter.SortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email);
                        break;
                    default:
                        query = filter.SortDescending ? query.OrderByDescending(u => u.Id) : query.OrderBy(u => u.Id);
                        break;
                }
            }
            else query = query.OrderBy(u => u.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query.ToListAsync(cancellationToken);

            List<UserResponseDTO> filteredList = new List<UserResponseDTO>();

            foreach (User user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userRole = roles.FirstOrDefault()?.ToLower() ?? "";

                if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                    if (userRole != filter.Role.ToLower()) continue;
                }

                filteredList.Add(new UserResponseDTO
                {
                    Id = user.Id,
                    Name = user.UserName,
                    Email = user.Email,
                    Role = userRole
                });
            }

            if (!string.IsNullOrWhiteSpace(filter.SortBy) && filter.SortBy.ToLower() == "role")
            {
                if (filter.SortDescending) filteredList.OrderByDescending(u => u.Role).ToList();
                else filteredList.OrderBy(u => u.Role).ToList();
            }

            var pagedItems = filteredList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

            return new PagedList<UserResponseDTO>(pagedItems, filteredList.Count, pageNumber, pageSize);
        }
    }
}
