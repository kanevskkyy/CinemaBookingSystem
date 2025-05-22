using CinemaBookingSystemDAL.Entities;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class SeatGeneration
    {
        public static List<Seat> Generate(CinemaDbContext context, List<Hall> hallList)
        {
            if (context.Seats.Any()) return context.Seats.ToList();

            List<Seat> seatList = new List<Seat>();

            foreach (var hall in hallList)
            {
                for (int row = 1; row <= hall.RowsAmount; row++)
                {
                    for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
                    {
                        seatList.Add(new Seat
                        {
                            HallId = hall.Id,
                            RowNumber = row,
                            SeatNumber = seat
                        });
                    }
                }
            }
            context.Seats.AddRange(seatList);
            context.SaveChanges();
            return seatList;
        }
    }
}
