using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Review
{
    public class ReviewResponseDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public int MovieId { get; set; }
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
