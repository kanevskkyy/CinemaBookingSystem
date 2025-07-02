using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class PaymentGeneration
    {
        public static async Task Generate(CinemaDbContext context, List<Ticket> tickets)
        {
            Faker faker = new Faker();
            List<Payment> payments = new List<Payment>();

            foreach (Ticket ticket in tickets.Where(t => t.IsPaid))
            {
                Payment payment = new Payment
                {
                    TicketId = ticket.Id,
                    Status = faker.PickRandom("Success", "Failed"),
                    TransactionId = faker.Finance.Iban(),
                    PaymentMethod = faker.PickRandom("Manual", "Card"),
                    PaymentDate = ticket.PurchaseTime,
                    CreatedAt = ticket.PurchaseTime
                };
                payments.Add(payment);
            }

            await context.Payments.AddRangeAsync(payments);
            await context.SaveChangesAsync();
        }
    }
}
