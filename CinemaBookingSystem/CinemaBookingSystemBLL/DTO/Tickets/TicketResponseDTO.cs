using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Tickets
{
    public class TicketResponseDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid SessionId { get; set; }
        public bool IsPaid { get; set; }
        public string SessionMovieTitle { get; set; }
        public Guid SeatId { get; set; }
        public string SeatInfo { get; set; }
        public DateTime PurchaseTime { get; set; }
    }
}
