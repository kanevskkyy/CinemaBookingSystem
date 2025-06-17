using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Exceptions
{
    public class EntityAlreadyExistsException : Exception
    {
        public EntityAlreadyExistsException(string entity, string column, string value) : base($"An entity '{entity}' with ColumnName '{column}' and value '{value}' already exists.") 
        {

        }
    }
}
