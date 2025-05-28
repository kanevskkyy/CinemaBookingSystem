using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Tickets
{
    public class TicketResponseDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int SessionId { get; set; }
        public bool IsPaid { get; set; }
        public string SessionMovieTitle { get; set; } = string.Empty;
        public int SeatId { get; set; }
        public string SeatInfo { get; set; } = string.Empty;
        public DateTime PurchaseTime { get; set; }
    }
}
