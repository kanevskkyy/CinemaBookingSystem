using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class InvalidSeatInHall : Exception
    {
        public InvalidSeatInHall() : base("The selected seat does not belong to the session's hall.") 
        {

        }
    }
}
