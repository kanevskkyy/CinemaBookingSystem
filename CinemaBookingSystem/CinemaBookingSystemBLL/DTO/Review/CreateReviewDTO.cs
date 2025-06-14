using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Review
{
    public class CreateReviewDTO
    {
        public Guid MovieId { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
    }
}
