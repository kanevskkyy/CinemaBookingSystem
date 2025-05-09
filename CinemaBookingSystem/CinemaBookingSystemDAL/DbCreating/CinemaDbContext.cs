using CinemaBookingSystemDAL.Configurations;
using CinemaBookingSystemDAL.DbCreating.DataGeneration;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating
{
    public class CinemaDbContext : DbContext
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options): base(options) {}
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new HallConfiguration());
            modelBuilder.ApplyConfiguration(new SeatConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            base.OnModelCreating(modelBuilder);
        }

        public static void Seed(CinemaDbContext context)
        {
            List<User> userList = UserGeneration.Generate(context);
            List<Genre> genreList = GenreGeneration.Generate(context);
            List<Hall> hallList = HallGeneration.Generate(context);
            List<Movie> movieList = MovieGeneration.Generate(context, genreList);
            List<Seat> seatList = SeatGeneration.Generate(context, hallList);
            List<Session> sessionList = SessionGeneration.Generate(context, movieList, hallList);
            SessionGeneration.Generate(context, movieList, hallList);
            TicketGeneration.Generate(context, userList, sessionList, seatList);
        }
    }
}
