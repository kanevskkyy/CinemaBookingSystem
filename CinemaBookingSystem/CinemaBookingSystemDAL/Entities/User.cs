using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class User
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Email { get; set; }
        public String Role { get; set; } = "Customer";
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
