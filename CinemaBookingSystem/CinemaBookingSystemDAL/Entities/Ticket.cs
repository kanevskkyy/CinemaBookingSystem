using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public Guid SeatId { get; set; }
        public DateTime PurchaseTime { get; set; }
        public bool IsPaid { get; set; } = false;
        public Session Session { get; set; }
        public DateTime? ReservedUntil { get; set; }
        public User User { get; set; }
        public Seat Seat { get; set; }

        public Payment Payment { get; set; }
    }
}
