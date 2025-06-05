using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Filters
{
    public class FilterReviewDto
    {
        public string? UserId { get; set; }
        public Guid? MovieId { get; set; }
        public int? MinRating { get; set; }
        public int? MaxRating { get; set; }
        public string? TextContains { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
    }
}
