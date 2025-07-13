using System.Threading.Tasks;
using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class TicketGeneration : IGenerateData
    {
        public async Task Generate(CinemaDbContext context)
        {
            if (!await context.Tickets.AnyAsync())
            {
                List<User> userList = await context.Users.ToListAsync();
                List<Session> sessionList = await context.Sessions.ToListAsync();
                List<Seat> seatList = await context.Seats.ToListAsync();

                Random random = new Random();
                List<Ticket> ticketList = new List<Ticket>();

                for (int i = 0; i < 100; i++)
                {
                    User user = userList[random.Next(userList.Count)];
                    Session session = sessionList[random.Next(sessionList.Count)];
                    Seat seat = seatList[random.Next(seatList.Count)];

                    DateTime purchaseTime = new Faker().Date.Between(DateTime.Now.ToUniversalTime(), session.StartTime.AddMinutes(-1).ToUniversalTime());
                    bool paid = new Faker().PickRandom(true, false);
                    Ticket tempTicket = new Ticket
                    {
                        UserId = user.Id,
                        SessionId = session.Id,
                        SeatId = seat.Id,
                        PurchaseTime = purchaseTime,
                        IsPaid = paid,
                    };

                    ticketList.Add(tempTicket);
                }

                await context.Tickets.AddRangeAsync(ticketList);
                await context.SaveChangesAsync();
            }
        }
    }

}
