using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class SessionGeneration
    {
        public static async Task<List<Session>> Generate(CinemaDbContext context, List<Movie> movieList, List<Hall> hallList)
        {
            if (await context.Sessions.AnyAsync()) return await context.Sessions.ToListAsync();

            DateTime timeNow = DateTime.Now.ToUniversalTime();
            Faker<Session> sessionFaker = new Faker<Session>("en")
                .RuleFor(p => p.MovieId, k => k.PickRandom(movieList).Id)
                .RuleFor(p => p.HallId, k => k.PickRandom(hallList).Id)
                .RuleFor(p => p.StartTime, k => k.Date.Between(timeNow, timeNow.AddMonths(1)).ToUniversalTime())
                .RuleFor(p => p.Price, k => k.Random.Number(100, 300));

            List<Session> sessions = sessionFaker.Generate(100);
            await context.Sessions.AddRangeAsync(sessions);
            await context.SaveChangesAsync();
            return sessions;
        }
    }
}
