using CinemaBookingSystemDAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemDAL.DbCreating.DataGeneration
{
    public class SeatGeneration : IGenerateData
    {
        public async Task Generate(CinemaDbContext context)
        {
            if(!await context.Seats.AnyAsync())
            {
                List<Hall> hallList = await context.Halls.ToListAsync();
                List<Seat> seatList = new List<Seat>();

                foreach (Hall hall in hallList)
                {
                    for (int row = 1; row <= hall.RowsAmount; row++)
                    {
                        for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
                        {
                            Seat tempSeat = new Seat
                            {
                                HallId = hall.Id,
                                RowNumber = row,
                                SeatNumber = seat
                            };
                            seatList.Add(tempSeat);
                        }
                    }
                }
                await context.Seats.AddRangeAsync(seatList);
                await context.SaveChangesAsync();
            }
        }
    }
}
