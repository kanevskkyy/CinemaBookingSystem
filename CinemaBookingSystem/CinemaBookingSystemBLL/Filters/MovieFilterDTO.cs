using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.Filters
{
    public class MovieFilterDTO
    {
        public string? Title { get; set; }
        public int? GenreId { get; set; }
        public double? MinRating { get; set; }         
        public double? MaxRating { get; set; } 
        public int? MinDuration { get; set; }  
        public int? MaxDuration { get; set; }


        public string? SortBy { get; set; } = "Id";
        public bool SortDescending { get; set; } = false;
    }
}
