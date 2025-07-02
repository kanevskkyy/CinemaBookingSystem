using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating
{
    public class CinemaDbContextFactory : IDesignTimeDbContextFactory<CinemaDbContext>
    {
        public CinemaDbContext CreateDbContext(string[] args)
        {
            DbContextOptionsBuilder<CinemaDbContext> optionsBuilder = new DbContextOptionsBuilder<CinemaDbContext>();

            string connection = "Host=localhost;Port=5432;Database=CinemaDb;Username=postgres;Password=postgres";
            optionsBuilder.UseNpgsql(connection);

            return new CinemaDbContext(optionsBuilder.Options);
        }
    }
}
