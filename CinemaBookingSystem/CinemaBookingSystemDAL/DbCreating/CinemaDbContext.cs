using CinemaBookingSystemDAL.Configurations;
using CinemaBookingSystemDAL.DbCreating.DataGeneration;
using CinemaBookingSystemDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;

namespace CinemaBookingSystemDAL.DbCreating
{
    public class CinemaDbContext : IdentityDbContext<User, Role, Guid>
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
        public DbSet<MovieGenre> MovieGenres { get; set; }
        public DbSet<Payment> Payments { get; set; }

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
            modelBuilder.ApplyConfiguration(new MovieGenreConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());

            base.OnModelCreating(modelBuilder);
        }

        public static async Task SeedAsync(CinemaDbContext context, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            await UserGeneration.GenerateAsync(userManager, roleManager);

            List<IGenerateData> generators = new List<IGenerateData>
            {
                new GenreGeneration(),
                new HallGeneration(),
                new MovieGeneration(),
                new SeatGeneration(),
                new SessionGeneration(),
                new TicketGeneration(),
                new ReviewGeneration(),
                new MovieGenreGeneration(),
                new PaymentGeneration()
            };

            foreach (IGenerateData generator in generators)
            {
                await generator.Generate(context);
            }
        }
    }
}