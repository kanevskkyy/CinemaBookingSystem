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
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.PurchaseTime)
                   .IsRequired();

            builder.Property(p => p.IsPaid)
                .IsRequired();

            builder.HasOne(p => p.User)
                   .WithMany(p => p.Tickets)
                   .HasForeignKey(p => p.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Session)
                   .WithMany(p => p.Ticket)
                   .HasForeignKey(p => p.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Seat)
                   .WithMany(p => p.Ticket)
                   .HasForeignKey(p => p.SeatId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
