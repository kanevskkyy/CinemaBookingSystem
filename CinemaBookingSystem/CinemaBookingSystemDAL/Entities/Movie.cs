using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Movie
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string PosterUrl { get; set; }
        public double Rating { get; set; }

        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
