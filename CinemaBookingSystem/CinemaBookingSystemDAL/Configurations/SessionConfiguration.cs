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
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(p => p.Id);
                
            builder.Property(p => p.StartTime)
                   .IsRequired();

            builder.Property(p => p.Price)
                   .IsRequired();

            builder.HasOne(p => p.Movie)
                   .WithMany(p => p.Sessions)
                   .HasForeignKey(p => p.MovieId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Hall)
                   .WithMany(p => p.Sessions)
                   .HasForeignKey(p => p.HallId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Ticket)
                   .WithOne(p => p.Session)
                   .HasForeignKey(p => p.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
