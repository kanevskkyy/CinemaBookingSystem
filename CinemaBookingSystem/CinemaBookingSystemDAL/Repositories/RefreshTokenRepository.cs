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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private CinemaDbContext context;

        public RefreshTokenRepository(CinemaDbContext context)
        {
            this.context = context;
        }

        public async Task<RefreshToken?> GetByUserIdAsync(string userId)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<RefreshToken?> GetByTokenAndUserIdAsync(string token, string userId)
        {
            return await context.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token && r.UserId == userId);
        }

        public async Task SaveAsync(string userId, RefreshToken refreshToken)
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
