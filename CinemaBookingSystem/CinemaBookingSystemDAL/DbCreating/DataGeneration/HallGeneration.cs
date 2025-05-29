using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class HallGeneration
    {
        public static List<Hall> Generate(CinemaDbContext context)
        {
            if (context.Halls.Any()) return context.Halls.ToList();

            var hallFaker = new Faker<Hall>("en")
                .RuleFor(p => p.RowsAmount, k => k.Random.Number(10, 30))
                .RuleFor(p => p.SeatsPerRow, k => k.Random.Number(15, 40));

            List<Hall> hallsList = new List<Hall>();
            int hallCount = 1;

            for (int i = 0; i < 10; i++)
            {
                Hall tempHall = new Hall
                {
                    Name = $"Hall {hallCount++}",
                    RowsAmount = hallFaker.Generate().RowsAmount,
                    SeatsPerRow = hallFaker.Generate().SeatsPerRow
                };
                hallsList.Add(tempHall);
            }

            context.Halls.AddRange(hallsList);
            context.SaveChanges();
            return hallsList;
        }
    }
}
