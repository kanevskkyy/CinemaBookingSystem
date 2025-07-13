using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public interface IGenerateData
    {
        Task Generate(CinemaDbContext context);
    }
}
