﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Movies
{
    public class MovieUpdateDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public string PosterUrl { get; set; }
        public List<Guid> GenreIds { get; set; }
        public double Rating { get; set; }
    }
}
