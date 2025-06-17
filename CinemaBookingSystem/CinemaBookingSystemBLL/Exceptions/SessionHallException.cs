using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class SessionHallException : Exception
    {
        public SessionHallException() : base("This hall already has a session during this time.") 
        {

        }
    }
}
