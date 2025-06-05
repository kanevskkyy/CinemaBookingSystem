using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CinemaBookingSystemDAL.Entities
{
    public class Review
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.ToUniversalTime();
        public int Rating { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }

        public Guid MovieId { get; set; }
        public Movie Movie { get; set; }
    }
}
