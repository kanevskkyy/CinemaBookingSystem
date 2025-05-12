using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Sessions
{
    public class SessionResponseDTO
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string MovieTitle { get; set; } = string.Empty;
        public int HallId { get; set; }
        public string HallName { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public int Price { get; set; }
    }
}
