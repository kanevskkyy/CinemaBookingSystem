using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Filters
{
    public class TicketFilterDTO
    {
        public string? UserId { get; set; }
        public int? SessionId { get; set; }
        public int? SeatId { get; set; }
        public DateTime? PurchaseTimeFrom { get; set; }
        public DateTime? PurchaseTimeTo { get; set; }

        public string SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
    }
}
