using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Authorization
{
    public class TokenRefreshRequest
    {
        public string AccessToken { get; set; } 
        public string RefreshToken { get; set; }
    }
}
