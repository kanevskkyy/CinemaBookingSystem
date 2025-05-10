using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class MovieGeneration
    {
        public static List<Movie> Generate(CinemaDbContext context, List<Genre> genreList)
        {
            if (context.Movies.Any()) return new List<Movie>();

            var movieFaker = new Faker<Movie>("en")
                .RuleFor(p => p.Title, k => k.Lorem.Sentence(3))
                .RuleFor(p => p.Description, k => k.Lorem.Paragraph())
                .RuleFor(p => p.Duration, k => k.Random.Number(60, 180))
                .RuleFor(p => p.PosterUrl, k => k.Image.PicsumUrl())
                .RuleFor(p => p.GenreId, f => f.PickRandom(genreList).Id)
                .RuleFor(p => p.Rating, f => f.Random.Double(2.0, 10.0));

            var movies = movieFaker.Generate(60);
            context.Movies.AddRange(movies);
            context.SaveChanges();
            return movies;
        }
    }
}
