using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemDAL.Interfaces.CinemaBookingSystemDAL.Interfaces;
using CinemaBookingSystemDAL.Interfaces;

namespace CinemaBookingSystemDAL.Unit_of_Work
{
    public interface IUnitOfWork : IDisposable
    {
        IMovieRepository Movies { get; }
        IGenreRepository Genres { get; }
        IHallRepository Halls { get; }
        ISeatRepository Seats { get; }
        ISessionRepository Sessions { get; }
        ITicketRepository Tickets { get; }
        IUserRepository Users { get; }
        IReviewRepository Review { get; }
        IRefreshTokenRepository RefreshTokens { get; }
        IPaymentRepository Payments { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
