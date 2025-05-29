using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class UserGeneration
    {
        public static async Task GenerateAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            string[] roles = new[] { "Admin", "Customer" };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role)) await roleManager.CreateAsync(new IdentityRole(role));
            }

            if (userManager.Users.Any()) return;

            var userFaker = new Faker<User>("en")
                .RuleFor(u => u.UserName, f => f.Internet.UserName())
                .RuleFor(u => u.Email, f => f.Internet.Email());

            var fakeUsers = userFaker.Generate(60);

            foreach (var user in fakeUsers)
            {
                string password = new Faker().Internet.Password(10, false, null, "!1Aa");

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    Random random = new Random();
                    int randomNumber = random.Next(0, 2);

                    var role = roles[randomNumber];
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
