using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class UserCreationFailedException : Exception
    {
        public UserCreationFailedException() : base("Failed to create new user") { }
    }
}
