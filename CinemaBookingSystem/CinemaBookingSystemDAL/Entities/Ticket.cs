using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Ticket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public int SeatId { get; set; }
        public DateTime PurchaseTime { get; set; }


        public Session Session { get; set; }
        public User User { get; set; }
        public Seat Seat { get; set; }
    }
}
