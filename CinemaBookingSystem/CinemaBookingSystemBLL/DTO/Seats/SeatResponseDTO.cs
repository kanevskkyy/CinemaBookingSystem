using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Seats
{
    public class SeatResponseDTO
    {
        public int Id { get; set; }
        public int HallId { get; set; }
        public int RowNumber { get; set; }
        public int SeatNumber { get; set; }
    }
}
