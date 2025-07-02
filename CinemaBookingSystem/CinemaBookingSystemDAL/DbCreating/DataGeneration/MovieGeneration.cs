using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class MovieGeneration
    {
        public static async Task<List<Movie>> Generate(CinemaDbContext context)
        {
            if (await context.Movies.AnyAsync()) return await context.Movies.ToListAsync();

            Faker<Movie> movieFaker = new Faker<Movie>("en")
                .RuleFor(p => p.Title, k => k.Lorem.Sentence(3))
                .RuleFor(p => p.Description, k => k.Lorem.Paragraph())
                .RuleFor(p => p.Duration, k => k.Random.Number(60, 180))
                .RuleFor(p => p.PosterUrl, k => k.Image.PicsumUrl())
                .RuleFor(p => p.Rating, f => Math.Round(f.Random.Double(2.0, 10.0), 1));

            List<Movie> movies = movieFaker.Generate(60);
            await context.Movies.AddRangeAsync(movies);
            await context.SaveChangesAsync();
            
            return movies;
        }
    }
}
