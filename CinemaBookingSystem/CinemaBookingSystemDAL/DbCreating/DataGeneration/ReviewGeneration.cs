using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public static class ReviewGeneration
    {
        public static async Task<List<Review>> Generate(CinemaDbContext context, List<User> users, List<Movie> movies)
        {
            if (context.Reviews.Any()) return await context.Reviews.ToListAsync();

            Faker faker = new Faker("en");
            Faker<Review> reviewFaker = new Faker<Review>("en")
                .RuleFor(r => r.Text, f => f.Lorem.Sentences(f.Random.Int(1, 3)))
                .RuleFor(r => r.CreatedAt, f => f.Date.Past(2).ToUniversalTime())
                .RuleFor(r => r.Rating, f => f.Random.Int(1, 5))
                .RuleFor(r => r.UserId, f => f.PickRandom(users).Id)
                .RuleFor(r => r.MovieId, f => f.PickRandom(movies).Id);

            List<Review> reviews = reviewFaker.Generate(200);
            await context.Reviews.AddRangeAsync(reviews);
            await context.SaveChangesAsync();

            return reviews;
        }
    }
}
