using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class UserPaymentsNotFoundException : Exception
    {
        public UserPaymentsNotFoundException(Guid userId) : base($"Cannot find any payment with this userID: {userId}")
        {

        }
    }
}
