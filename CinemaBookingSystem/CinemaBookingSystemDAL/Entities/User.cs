﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CinemaBookingSystemDAL.Entities
{
    public class User : IdentityUser
    {
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
