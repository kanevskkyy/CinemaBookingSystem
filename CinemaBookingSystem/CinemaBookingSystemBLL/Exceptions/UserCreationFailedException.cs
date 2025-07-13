using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class UserCreationFailedException : Exception
    {
        public UserCreationFailedException()
        {

        }

        public UserCreationFailedException(string message) : base(message)
        {

        }

        public UserCreationFailedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
