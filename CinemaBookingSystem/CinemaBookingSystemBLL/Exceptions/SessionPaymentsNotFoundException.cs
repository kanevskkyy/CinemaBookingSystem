using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class SessionPaymentsNotFoundException : Exception
    {
        public SessionPaymentsNotFoundException(Guid sessionId) : base($"Cannot find any payment with this sessionID: {sessionId}")
        {

        }
    }
}
