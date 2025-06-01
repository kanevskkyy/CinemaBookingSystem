using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface ISeatRepository : IGenericRepository<Seat, int>
    {
        Task<List<Seat>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default);
        Task<Seat?> GetByRowAndNumberAsync(int hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken = default);
    }
}
