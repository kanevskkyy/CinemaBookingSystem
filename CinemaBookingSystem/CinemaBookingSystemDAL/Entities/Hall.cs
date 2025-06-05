using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Hall
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int RowsAmount { get; set; }
        public int SeatsPerRow { get; set; }
        public ICollection<Seat> Seats { get; set; } = new List<Seat>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
