using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Users
{
    public class UserResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        public string Email { get; set; }
        public string Role { get; set; } 
    }
}