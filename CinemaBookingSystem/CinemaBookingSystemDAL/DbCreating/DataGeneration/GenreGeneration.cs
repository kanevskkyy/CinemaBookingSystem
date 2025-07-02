using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class GenreGeneration
    {
        public static async Task<List<Genre>> Generate(CinemaDbContext context)
        {
            if (await context.Genres.AnyAsync()) return await context.Genres.ToListAsync();

            List<Genre> genreList = new List<Genre>();
            List<string> genreTypes = new List<string> { "Action", "Comedy", "Drama", "Horror", "Science Fiction", "Romance", "Thriller", "Fantasy", "Adventure", "Documentary" };
            foreach (string genreName in genreTypes)
            {
                Genre tempGenre = new Genre
                {
                    Name = genreName
                };
                genreList.Add(tempGenre);
            }

            await context.Genres.AddRangeAsync(genreList);
            await context.SaveChangesAsync();
            return genreList;
        }
    }
}
