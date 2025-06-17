using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class SessionGeneration
    {
        public static List<Session> Generate(CinemaDbContext context, List<Movie> movieList, List<Hall> hallList)
        {
            if (context.Sessions.Any()) return context.Sessions.ToList();

            DateTime timeNow = DateTime.Now;
            DateTime localTime = timeNow.ToUniversalTime();
            Faker<Session> sessionFaker = new Faker<Session>("en")
                .RuleFor(p => p.MovieId, k => k.PickRandom(movieList).Id)
                .RuleFor(p => p.HallId, k => k.PickRandom(hallList).Id)
                .RuleFor(p => p.StartTime, k => k.Date.Between(localTime, localTime.AddMonths(1)).ToUniversalTime())
                .RuleFor(p => p.Price, k => k.Random.Number(100, 300));

            var sessions = sessionFaker.Generate(100);
            context.Sessions.AddRange(sessions);
            context.SaveChanges();
            return sessions;
        }
    }
}
