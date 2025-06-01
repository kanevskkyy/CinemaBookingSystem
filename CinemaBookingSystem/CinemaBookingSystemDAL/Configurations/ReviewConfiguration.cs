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
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Text)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasIndex(p => p.UserId);

            builder.HasIndex(p => p.CreatedAt);

            builder.HasIndex(p => p.Text);

            builder.HasIndex(p => p.Rating);

            builder.HasIndex(p => p.MovieId);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.Rating)
                .IsRequired()
                .HasDefaultValue(1);

            builder.HasOne(p => p.User)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Movie)
                .WithMany(p => p.Reviews)
                .HasForeignKey(p => p.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasCheckConstraint("CK_Review_Rating_Range", "\"Rating\" >= 1 AND \"Rating\" <= 5");
        }
    }
}
