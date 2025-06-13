using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Genres;

namespace CinemaBookingSystemBLL.DTO.Movies
{
    public class MovieResponseDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } 
        public int Duration { get; set; }
        public string PosterUrl { get; set; }
        public List<GenreResponseDTO> Genres { get; set; }
        public double Rating { get; set; }
    }
}
