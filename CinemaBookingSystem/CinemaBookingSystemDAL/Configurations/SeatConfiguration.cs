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
    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.RowNumber)
                   .IsRequired();

            builder.Property(p => p.SeatNumber)
                   .IsRequired();

            builder.HasOne(p => p.Hall)
                   .WithMany(p => p.Seats)
                   .HasForeignKey(p => p.HallId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Ticket)
                   .WithOne(p => p.Seat)
                   .HasForeignKey(p => p.SeatId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
