using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Review
{
    public class CreateReviewDTO
    {
        public string UserId { get; set; } = null!;
        public int MovieId { get; set; }
        public string Text { get; set; } = null!;
        public int Rating { get; set; }
    }
}
