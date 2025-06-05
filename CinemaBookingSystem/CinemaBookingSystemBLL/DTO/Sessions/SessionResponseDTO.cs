using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Sessions
{
    public class SessionResponseDTO
    {
        public Guid Id { get; set; }
        public Guid MovieId { get; set; }
        public string MovieTitle { get; set; }
        public Guid HallId { get; set; }
        public string HallName { get; set; } 
        public DateTime StartTime { get; set; }
        public int Price { get; set; }
    }
}
