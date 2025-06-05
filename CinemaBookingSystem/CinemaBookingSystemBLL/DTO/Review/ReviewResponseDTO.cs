using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Review
{
    public class ReviewResponseDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public Guid MovieId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
