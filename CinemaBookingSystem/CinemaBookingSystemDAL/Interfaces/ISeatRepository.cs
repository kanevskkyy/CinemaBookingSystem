using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat>
    {
        Task<List<Seat>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default);
    }
}
