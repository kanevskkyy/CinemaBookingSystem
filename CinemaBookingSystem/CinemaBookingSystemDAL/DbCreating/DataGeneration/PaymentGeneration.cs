using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class PaymentGeneration : IGenerateData
    {
        public async Task Generate(CinemaDbContext context)
        {
            if(!await context.Payments.AnyAsync())
            {
                Faker faker = new Faker();
                List<Payment> payments = new List<Payment>();
                List<Ticket> tickets = await context.Tickets.ToListAsync();
                tickets = tickets.Where(p => p.IsPaid).ToList();

                foreach (Ticket ticket in tickets)
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
}
