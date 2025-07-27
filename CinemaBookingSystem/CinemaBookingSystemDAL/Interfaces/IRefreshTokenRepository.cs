using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByUserIdAsync(Guid userId);
        Task<RefreshToken?> GetByTokenAndUserIdAsync(string token, Guid userId);
        Task SaveAsync(Guid userId, RefreshToken refreshToken);
    }
}
