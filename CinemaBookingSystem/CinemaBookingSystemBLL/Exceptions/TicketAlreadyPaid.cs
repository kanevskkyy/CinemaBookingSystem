using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class TicketAlreadyPaid : Exception
    {
        public TicketAlreadyPaid() : base("This ticket is already paid!")
        {
            
        }
    }
}
