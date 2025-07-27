using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class ReviewAlreadyExistsException : Exception
    {
        public ReviewAlreadyExistsException(Guid userId, Guid movieId) : base($"User with ID: {userId} has already left review about movie with ID: {movieId}") 
        {

        }
    }
}
