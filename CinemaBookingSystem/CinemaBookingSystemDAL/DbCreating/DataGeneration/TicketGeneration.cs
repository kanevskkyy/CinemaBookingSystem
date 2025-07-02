using System.Threading.Tasks;
using Bogus;
using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class TicketGeneration
    {
        public static async Task<List<Ticket>> Generate(CinemaDbContext context, List<User> userList, List<Session> sessionList, List<Seat> seatList)
        {
            if (await context.Tickets.AnyAsync()) return await context.Tickets.ToListAsync();

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
            return ticketList;
        }
    }

}
