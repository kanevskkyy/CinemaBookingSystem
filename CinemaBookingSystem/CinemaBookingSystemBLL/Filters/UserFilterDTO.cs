using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Filters
{
    public class UserFilterDTO
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }

        public string SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
    }
}
