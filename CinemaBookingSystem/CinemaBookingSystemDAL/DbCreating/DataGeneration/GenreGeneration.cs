using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class GenreGeneration : IGenerateData
    {
        public async Task Generate(CinemaDbContext context)
        {
            if(!await context.Genres.AnyAsync())
            {
                List<string> genreTypes = new List<string> { "Action", "Comedy", "Drama", "Horror", "Science Fiction", "Romance", "Thriller", "Fantasy", "Adventure", "Documentary" };
                List<Genre> genreList = new List<Genre>();

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
            }
        }
    }
}
