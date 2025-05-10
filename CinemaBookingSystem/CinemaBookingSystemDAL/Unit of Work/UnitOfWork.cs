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
        private readonly CinemaDbContext _context;

        public UnitOfWork(CinemaDbContext context)
        {
            _context = context;
        }

        public IMovieRepository Movies => new MovieRepository(_context);
        public IGenreRepository Genres => new GenreRepository(_context);
        public IHallRepository Halls => new HallRepository(_context);
        public ISeatRepository Seats => new SeatRepository(_context);
        public ISessionRepository Sessions => new SessionRepository(_context);
        public ITicketRepository Tickets => new TicketRepository(_context);
        public IUserRepository Users => new UserRepository(_context);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
