using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IGenreRepository : IGenericRepository<Genre>
    {
        Task<Dictionary<string, int>> GetMovieCountsPerGenreAsync(CancellationToken cancellationToken = default);
        Task<bool> ExistsByNameAsync(string name, Guid? id = null, CancellationToken cancellationToken = default);
    }
}