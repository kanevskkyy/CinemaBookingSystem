using CinemaBookingSystemDAL.Configurations;
using CinemaBookingSystemDAL.DbCreating.DataGeneration;
using CinemaBookingSystemDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace CinemaBookingSystemDAL.DbCreating
{
    public class CinemaDbContext : IdentityDbContext<User>
    {
        public CinemaDbContext(DbContextOptions<CinemaDbContext> options) : base(options)
        {

        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Hall> Halls { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new HallConfiguration());
            modelBuilder.ApplyConfiguration(new SeatConfiguration());
            modelBuilder.ApplyConfiguration(new SessionConfiguration());
            modelBuilder.ApplyConfiguration(new TicketConfiguration());
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());
            modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public static async Task SeedAsync(CinemaDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            await UserGeneration.GenerateAsync(userManager, roleManager);
            List<Genre> genres = GenreGeneration.Generate(context);
            List<Hall> halls = HallGeneration.Generate(context);
            List<Movie> movies = MovieGeneration.Generate(context, genres);
            List<Seat> seats = SeatGeneration.Generate(context, halls);
            List<Session> sessions = SessionGeneration.Generate(context, movies, halls);
            List<User> users = userManager.Users.ToList();
            List<Review> reviews = ReviewGeneration.Generate(context, users, movies);
            TicketGeneration.Generate(context, users, sessions, seats);
            await context.SaveChangesAsync();
        }
    }
}