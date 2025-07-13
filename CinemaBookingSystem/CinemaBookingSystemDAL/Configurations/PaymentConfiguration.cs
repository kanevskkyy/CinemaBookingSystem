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
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasCheckConstraint("CK_Payment_Status", "Status IN ('Success', 'Failed')");
            builder.HasCheckConstraint("CK_Payment_PaymentMethod", "PaymentMethod IN ('Manual', 'Card')");

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(p => p.Ticket)
               .WithOne(t => t.Payment)
               .HasForeignKey<Payment>(p => p.TicketId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
