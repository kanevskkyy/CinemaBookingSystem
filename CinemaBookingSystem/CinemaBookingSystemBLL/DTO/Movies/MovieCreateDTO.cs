using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Movies
{
    public class MovieCreateDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string PosterUrl { get; set; } = string.Empty;
        public int GenreId { get; set; }
        public double Rating { get; set; }
    }
}
