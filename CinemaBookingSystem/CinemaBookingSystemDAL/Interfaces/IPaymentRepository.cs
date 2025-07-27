using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.Interfaces
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<List<Payment>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<List<Payment>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task<List<Payment>> GetAllWithDetailsAsync(CancellationToken cancellationToken = default);
    }
}
