using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Filters
{
    public class SessionFilterDTO
    {
        public Guid? MovieId { get; set; }
        public Guid? HallId { get; set; }
        public DateTime? StartTimeFrom { get; set; }
        public DateTime? StartTimeTo { get; set; }
        public int? MinPrice { get; set; }
        public int? MaxPrice { get; set; }

        public string SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
    }
}
