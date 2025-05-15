using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemBLL.DTO.Halls
{
    public class HallCreateDTO
    {
        public string Name { get; set; }
        public int RowAmount {  get; set; }
        public int SeatsPerRow { get; set; }
    }
}
