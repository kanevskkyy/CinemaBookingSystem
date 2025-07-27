using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Repositories
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {

        public RefreshTokenRepository(CinemaDbContext context) : base(context)
        {

        }

        public async Task<RefreshToken?> GetByUserIdAsync(Guid userId)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<RefreshToken?> GetByTokenAndUserIdAsync(string token, Guid userId)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token && r.UserId == userId);
        }

        public async Task SaveAsync(Guid userId, RefreshToken refreshToken)
        {
            RefreshToken? existing = await GetByUserIdAsync(userId);
            
            if (existing != null)
            {
                existing.Token = refreshToken.Token;
                existing.ExpiryDate = refreshToken.ExpiryDate;
            }
            
            else
            {
                refreshToken.UserId = userId;
                await context.RefreshTokens.AddAsync(refreshToken);
            }

            await context.SaveChangesAsync();
        }
    }
}
