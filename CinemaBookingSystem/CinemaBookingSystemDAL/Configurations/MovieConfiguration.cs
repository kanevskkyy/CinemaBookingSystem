using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CinemaBookingSystemDAL.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .HasMaxLength(90)
                .IsRequired();

            builder.HasIndex(p => p.Title)
                .IsUnique();

            builder.HasIndex(p => p.GenreId);

            builder.HasIndex(p => p.Duration);
            
            builder.HasIndex(p => p.Rating);

            builder.Property(p => p.Description)
                .HasMaxLength(3000)
                .IsRequired();

            builder.Property(p => p.Duration)
                .IsRequired();

            builder.Property(p => p.PosterUrl)
                .IsRequired();

            builder.HasMany(p => p.Sessions)
                .WithOne(p => p.Movie)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Genre)
            .WithMany(p => p.Movies)
            .HasForeignKey(p => p.GenreId)
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Rating)
                .IsRequired();
        }
    }
}
