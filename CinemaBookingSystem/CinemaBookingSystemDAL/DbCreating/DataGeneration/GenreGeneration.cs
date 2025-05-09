using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class GenreGeneration
    {
        public static List<Genre> Generate(CinemaDbContext context)
        {
            if (context.Genres.Any()) return new List<Genre>();

            List<Genre> genreList = new List<Genre>();
            List<string> genreTypes = new List<string> { "Action", "Comedy", "Drama", "Horror", "Science Fiction", "Romance", "Thriller", "Fantasy", "Adventure", "Documentary" };
            foreach (var genreName in genreTypes)
            {
                genreList.Add(new Genre
                {
                    Name = genreName
                });
            }
            context.Genres.AddRange(genreList);
            context.SaveChanges();
            return genreList;
        }
    }
}
