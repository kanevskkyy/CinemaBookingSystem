using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class MovieGenreGeneration
    {
        public static void Generate(CinemaDbContext context, List<Movie> movies, List<Genre> genres)
        {
            if (context.MovieGenres.Any()) return;

            List<MovieGenre> movieGenres = new List<MovieGenre>();
            Faker faker = new Faker();

            foreach (Movie movie in movies)
            {
                int genresNumber = faker.Random.Int(1, genres.Count);
                List<Genre> randomGenres = faker.Random.Shuffle(genres).Take(genresNumber).ToList();

                foreach (Genre genre in randomGenres)
                {
                    MovieGenre movieGenre = new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = genre.Id
                    };
                    movieGenres.Add(movieGenre);
                }
            }
            context.MovieGenres.AddRange(movieGenres);
        }
    }
}