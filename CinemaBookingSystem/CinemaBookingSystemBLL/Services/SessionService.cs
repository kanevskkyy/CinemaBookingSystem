using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.Sessions.GetAllAsync(cancellationToken);
            return sessions.Select(p => new SessionResponseDTO { Id = p.Id, MovieId = p.MovieId, MovieTitle = p.Movie.Title, HallId = p.HallId, HallName = p.Hall.Name, StartTime = p.StartTime, Price = p.Price }).ToList();
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);

            if (session == null) return null;
            else return new SessionResponseDTO { Id = session.Id, MovieId = session.MovieId, MovieTitle = session.Movie.Title, HallId = session.HallId, HallName = session.Hall.Name, StartTime = session.StartTime,  Price = session.Price };
        }

        public async Task<List<SessionResponseDTO>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.Sessions.GetByMovieIdAsync(movieId, cancellationToken);
            return sessions.Select(p => new SessionResponseDTO { Id = p.Id, MovieId = p.MovieId, MovieTitle = p.Movie.Title, HallId = p.HallId, HallName = p.Hall.Name, StartTime = p.StartTime, Price = p.Price }).ToList();
        }

        public async Task<List<SessionResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.Sessions.GetByHallIdAsync(hallId, cancellationToken);
            return sessions.Select(p => new SessionResponseDTO { Id = p.Id, MovieId = p.MovieId, MovieTitle = p.Movie.Title, HallId = p.HallId, HallName = p.Hall.Name, StartTime = p.StartTime, Price = p.Price }).ToList();
        }

        public async Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var sessions = await _unitOfWork.Sessions.GetByDateRangeAsync(startDate, endDate, cancellationToken);
            return sessions.Select(p => new SessionResponseDTO { Id = p.Id, MovieId = p.MovieId, MovieTitle = p.Movie.Title, HallId = p.HallId, HallName = p.Hall.Name, StartTime = p.StartTime, Price = p.Price }).ToList();
        }

        public async Task<SessionResponseDTO> CreateAsync(SessionCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Session session = new Session{ MovieId = dto.MovieId, HallId = dto.HallId, StartTime = dto.StartTime, Price = dto.Price };
            await _unitOfWork.Sessions.CreateAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SessionResponseDTO { Id = session.Id, MovieId = session.MovieId, MovieTitle = session.Movie.Title, HallId = session.HallId, HallName = session.Hall.Name, StartTime = session.StartTime, Price = session.Price};
        }

        public async Task<SessionResponseDTO?> UpdateAsync(int id, SessionUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;

            session.StartTime = dto.StartTime;
            session.Price = dto.Price;

            _unitOfWork.Sessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SessionResponseDTO { Id = session.Id, MovieId = session.MovieId, MovieTitle = session.Movie.Title, HallId = session.HallId, HallName = session.Hall.Name, StartTime = session.StartTime, Price = session.Price };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return false;

            _unitOfWork.Sessions.Delete(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
