using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IHallRepository : IGenericRepository<Hall>
    {
        Task<Hall?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
