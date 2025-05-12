using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Tickets
{
    public class TicketCreateDTO
    {
        public int UserId { get; set; }
        public int SessionId { get; set; }
        public int SeatId { get; set; }
    }
}
