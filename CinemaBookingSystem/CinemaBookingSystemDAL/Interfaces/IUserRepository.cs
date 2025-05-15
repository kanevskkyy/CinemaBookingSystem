using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<List<User>> GetAllWithTicketsAsync(CancellationToken cancellationToken = default);
        Task<User?> GetWithTicketsAsync(int userId, CancellationToken cancellationToken = default);
        Task<PagedList<User>> GetPagedUsersAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    }
}
