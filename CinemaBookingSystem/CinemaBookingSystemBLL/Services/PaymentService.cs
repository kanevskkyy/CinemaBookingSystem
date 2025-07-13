using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Payment;
using CinemaBookingSystemBLL.Exceptions;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class PaymentService : IPaymentService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<PaymentResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<Payment> payments = await unitOfWork.Payments.GetAllWithDetailsAsync(cancellationToken);
            
            return mapper.Map<List<PaymentResponseDTO>>(payments);
        }

        public async Task<List<PaymentResponseDTO>> GetBySessionIdAsync(Guid sessionId, CancellationToken cancellationToken = default)
        {
            List<Payment> payments = await unitOfWork.Payments.GetBySessionIdAsync(sessionId, cancellationToken);
            if (!payments.Any()) throw new SessionPaymentsNotFoundException(sessionId);

            return mapper.Map<List<PaymentResponseDTO>>(payments);
        }

        public async Task<List<PaymentResponseDTO>> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            List<Payment> payments = await unitOfWork.Payments.GetByUserIdAsync(userId, cancellationToken);
            if (!payments.Any()) throw new UserPaymentsNotFoundException(userId);

            return mapper.Map<List<PaymentResponseDTO>>(payments);
        }
    }
}
