using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Payment;

namespace CinemaBookingSystemBLL.Interfaces
{
    public interface IPaymentService
    {
        Task<List<PaymentResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<PaymentResponseDTO>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default);
        Task<List<PaymentResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    }
}
