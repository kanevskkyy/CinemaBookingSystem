using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string Status { get; set; } = "Success";
        public string TransactionId { get; set; }
        public string PaymentMethod { get; set; } = "Manual";
        public DateTime? PaymentDate { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToUniversalTime();
    }
}