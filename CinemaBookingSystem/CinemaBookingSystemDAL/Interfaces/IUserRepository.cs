﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IUserRepository : IGenericRepository<User, string>
    {
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<List<User>> GetAllWithTicketsAsync(CancellationToken cancellationToken = default);
        Task<User?> GetWithTicketsAsync(string userId, CancellationToken cancellationToken = default);
    }
}