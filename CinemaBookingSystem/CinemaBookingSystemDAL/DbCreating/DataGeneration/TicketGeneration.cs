using Bogus;
using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class TicketGeneration
    {
        public static void Generate(CinemaDbContext context, List<User> userList, List<Session> sessionList, List<Seat> seatList)
        {
            if (context.Tickets.Any()) return;

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

            context.Tickets.AddRange(ticketList);
            context.SaveChanges();
        }
    }

}
