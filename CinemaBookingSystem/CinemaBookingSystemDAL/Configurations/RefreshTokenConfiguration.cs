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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Token)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(u => u.ExpiryDate)
                   .IsRequired();

            builder.Property(u => u.UserId)
                   .IsRequired();

            builder.HasIndex(u => u.UserId);

            builder.HasOne(u => u.User)            
                   .WithMany(u => u.RefreshTokens)
                   .HasForeignKey(u => u.UserId)
                   .OnDelete(DeleteBehavior.Cascade);    
        }
    }
}
