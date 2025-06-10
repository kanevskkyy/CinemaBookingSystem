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
    public class HallConfiguration : IEntityTypeConfiguration<Hall>
    {
        public void Configure(EntityTypeBuilder<Hall> builder)
        {
            builder.HasKey(p => p.Id);
            
            builder.Property(p => p.Name)
                .HasMaxLength(60)
                .IsRequired();

            builder.HasIndex(p => p.Name)
                .IsUnique();

            builder.Property(p => p.RowsAmount)
                .IsRequired();

            builder.Property(p => p.SeatsPerRow)
                .IsRequired();

            builder.HasMany(p => p.Seats)
                .WithOne(p => p.Hall)
                .HasForeignKey(p => p.HallId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasCheckConstraint("CK_Hall_RowsAmount", "\"RowsAmount\" >= 1");

            builder.HasCheckConstraint("CK_Hall_SeatsPerRow", "\"SeatsPerRow\" >= 1");

            builder.HasMany(p => p.Sessions)
                .WithOne(p => p.Hall)
                .HasForeignKey(p => p.HallId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
