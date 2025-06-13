using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.Configurations
{
    public class MovieGenreConfiguration : IEntityTypeConfiguration<MovieGenre>
    {
        public void Configure(EntityTypeBuilder<MovieGenre> builder)
        {
            builder.HasKey(p => new { p.MovieId, p.GenreId });

            builder.HasOne(p => p.Movie)
                .WithMany(p => p.MovieGenres)
                .HasForeignKey(p => p.MovieId);

            builder.HasOne(p => p.Genre)
                .WithMany(p => p.MovieGenres)
                .HasForeignKey(p => p.GenreId);
        }
    }
}
