using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.DbCreating;
using CinemaBookingSystemDAL.Interfaces.CinemaBookingSystemDAL.Interfaces;
using CinemaBookingSystemDAL.Interfaces;
using CinemaBookingSystemDAL.Repositories;

namespace CinemaBookingSystemDAL.Unit_of_Work
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CinemaDbContext context;

        public UnitOfWork(CinemaDbContext context)
        {
            this.context = context;
        }

        public IMovieRepository Movies => new MovieRepository(context);
        public IGenreRepository Genres => new GenreRepository(context);
        public IHallRepository Halls => new HallRepository(context);
        public ISeatRepository Seats => new SeatRepository(context);
        public ISessionRepository Sessions => new SessionRepository(context);
        public ITicketRepository Tickets => new TicketRepository(context);
        public IUserRepository Users => new UserRepository(context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
