using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Session
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MovieId { get; set; }
        public Guid HallId { get; set; }
        public DateTime StartTime { get; set; }
        public int Price { get; set; }  

        public Movie Movie { get; set; }
        public Hall Hall { get; set; }

        public ICollection<Ticket> Ticket { get; set; } = new List<Ticket>();
    }
}
