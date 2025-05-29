using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class GenreGeneration
    {
        public static List<Genre> Generate(CinemaDbContext context)
        {
            if (context.Genres.Any()) context.Genres.ToList();

            List<Genre> genreList = new List<Genre>();
            List<string> genreTypes = new List<string> { "Action", "Comedy", "Drama", "Horror", "Science Fiction", "Romance", "Thriller", "Fantasy", "Adventure", "Documentary" };
            foreach (var genreName in genreTypes)
            {
                Genre tempGenre = new Genre
                {
                    Name = genreName
                };
                genreList.Add(tempGenre);
            }
            context.Genres.AddRange(genreList);
            context.SaveChanges();
            return genreList;
        }
    }
}
