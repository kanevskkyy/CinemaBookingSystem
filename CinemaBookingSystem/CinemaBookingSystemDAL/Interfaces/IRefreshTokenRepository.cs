using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByUserIdAsync(string userId);
        Task<RefreshToken?> GetByTokenAndUserIdAsync(string token, string userId);
        Task SaveAsync(string userId, RefreshToken refreshToken);
    }
}
