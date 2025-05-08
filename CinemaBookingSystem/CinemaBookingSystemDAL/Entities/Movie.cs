using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.Entities
{
    public class Movie
    {
        public int Id { get; set; }
        public String Title { get; set; }
        public String Description { get; set; }
        public int Duration { get; set; }
        public String PosterUrl { get; set; }
        public ICollection<Session> Sessions { get; set; } = new List<Session>();

    }
}
