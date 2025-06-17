using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, Guid Id) : base($"{entityName} not found with ID: {Id}") 
        {

        }
    }
}