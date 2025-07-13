using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Payment
{
    public class PaymentResponseDTO
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public string? Status { get; set; }
        public string? TransactionId { get; set; } 
        public string? PaymentMethod { get; set; } 
        public DateTime? PaymentDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid SessionId { get; set; }
        public string? UserId { get; set; }
    }
}
