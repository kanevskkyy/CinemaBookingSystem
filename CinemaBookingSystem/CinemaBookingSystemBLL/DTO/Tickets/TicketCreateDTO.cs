using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Tickets
{
    public class TicketCreateDTO
    {
        public Guid SessionId { get; set; }
        public Guid SeatId { get; set; }
    }
}
