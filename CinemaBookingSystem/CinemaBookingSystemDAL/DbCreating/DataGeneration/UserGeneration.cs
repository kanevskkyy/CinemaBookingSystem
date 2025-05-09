using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class UserGeneration
    {
        public static List<User> Generate(CinemaDbContext context)
        {
            if (context.Users.Any()) return new List<User>();

            var userFaker = new Faker<User>("en")
                .RuleFor(p => p.Name, f => f.Person.FullName)
                .RuleFor(p => p.Email, f => f.Person.Email)
                .RuleFor(p => p.Password, f => f.Internet.Password())
                .RuleFor(p => p.Role, f => f.PickRandom("Customer", "Admin"));

            var users = userFaker.Generate(60);
            context.Users.AddRange(users);
            context.SaveChanges();
            return users;
        }
    }

}
