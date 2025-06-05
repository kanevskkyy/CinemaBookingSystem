using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Seat
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid HallId { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
        public Hall Hall { get; set; }
        public ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();
    }
}
