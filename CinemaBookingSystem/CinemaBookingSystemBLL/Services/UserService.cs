using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;
using Microsoft.EntityFrameworkCore;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.AspNetCore.Identity;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemBLL.Filters;
using AutoMapper;

namespace CinemaBookingSystemBLL.Services
{
    public class UserService : IUserService
    {
        private IUnitOfWork unitOfWork;
        private UserManager<User> userManager;
        private IMapper mapper;

        public UserService(IUnitOfWork unitOfWork, UserManager<User> userManager, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.userManager = userManager;
            this.mapper = mapper;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDTO dto, CancellationToken cancellationToken)
        {
            User user = await userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var result = await userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            return result.Succeeded;
        }

        public async Task<PagedList<UserResponseDTO>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Users.GetAll();
            PagedList<User> pagedUsers = await PagedList<User>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            List<User> userList = pagedUsers.Items;

            List<UserResponseDTO> result = new List<UserResponseDTO>();
            foreach (User user in userList)
            {
                var roles = await userManager.GetRolesAsync(user);
                string role = roles.FirstOrDefault();

                UserResponseDTO temp = mapper.Map<UserResponseDTO>(user);
                temp.Role = role;
                result.Add(temp);
            }

            return new PagedList<UserResponseDTO>(result, pagedUsers.TotalCount, pagedUsers.CurrentPage, pagedUsers.PageSize);
        }

        public async Task<List<UserResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var users = await unitOfWork.Users.GetAllAsync(cancellationToken);

            List<UserResponseDTO> result = new List<UserResponseDTO>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                string role = roles.FirstOrDefault();

                UserResponseDTO temp = mapper.Map<UserResponseDTO>(user);
                temp.Role = role;
                result.Add(temp);
            }
            return result;
        }

        public async Task<UserResponseDTO?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
        {
            User user = await unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) return null;

            var roles = await userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();

            UserResponseDTO result = mapper.Map<UserResponseDTO>(user);
            result.Role = role;

            return result;
        }


        public async Task<bool> DeleteAsync(string id, CancellationToken cancellationToken = default)
        {
            User user = await unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) return false;

            unitOfWork.Users.Delete(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<UserResponseDTO?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            User user = await unitOfWork.Users.GetByEmailAsync(email, cancellationToken);
            if (user == null) return null;

            var roles = await userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();

            UserResponseDTO result = mapper.Map<UserResponseDTO>(user);
            result.Role = role;

            return result;
        }

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await unitOfWork.Users.ExistsByEmailAsync(email, cancellationToken);
        }
        public async Task<UserResponseDTO> UpdateAsync(string id, UserUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            User user = await unitOfWork.Users.GetByIdAsync(id, cancellationToken);
            if (user == null) throw new KeyNotFoundException("User not found");

            user.UserName = dto.Name;

            unitOfWork.Users.Update(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var roles = await userManager.GetRolesAsync(user);
            string role = roles.FirstOrDefault();

            UserResponseDTO result = mapper.Map<UserResponseDTO>(user);
            result.Role = role;

            return result;
        }


        public async Task<PagedList<UserResponseDTO>> GetFilteredUsersAsync(UserFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<User> query = unitOfWork.Users.GetAll();

            if (!string.IsNullOrWhiteSpace(filter.Name)) query = query.Where(u => u.UserName.ToLower().Contains(filter.Name.ToLower()));
            if (!string.IsNullOrWhiteSpace(filter.Email)) query = query.Where(u => u.Email.ToLower().Contains(filter.Email.ToLower()));
           
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

            int totalCount = await query.CountAsync(cancellationToken);
            List<User> users = await query.ToListAsync(cancellationToken);

            List<UserResponseDTO> filteredList = new List<UserResponseDTO>();

            foreach (User user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                string userRole = roles.FirstOrDefault()?.ToLower();

                if (!string.IsNullOrWhiteSpace(filter.Role))
                {
                    if (userRole != filter.Role.ToLower()) continue;
                }

                UserResponseDTO temp = mapper.Map<UserResponseDTO>(user);
                temp.Role = userRole;

                filteredList.Add(temp);
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
