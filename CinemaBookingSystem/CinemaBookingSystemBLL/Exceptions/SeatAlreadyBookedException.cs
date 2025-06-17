using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class SeatAlreadyBookedException : Exception
    {
        public SeatAlreadyBookedException() : base("This seat is already booked for the selected session.") 
        {

        }
    }
}
