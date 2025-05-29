using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemBLL.Services
{
    public class SessionService : ISessionService
    {
        private IUnitOfWork unitOfWork;

        public SessionService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        private async Task<string> GetMovieTitleAsync(int movieId, CancellationToken cancellationToken)
        {
            var movie = await unitOfWork.Movies.GetByIdAsync(movieId, cancellationToken);
            return movie.Title;
        }

        public async Task<PagedList<SessionResponseDTO>> GetPagedSessionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var pagedSessions = await unitOfWork.Sessions.GetPagedSessionsAsync(pageNumber, pageSize, cancellationToken);
            List<SessionResponseDTO> sessionDtos = new List<SessionResponseDTO>();

            foreach (var session in pagedSessions)
            {
                var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
                var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
                SessionResponseDTO temp = new SessionResponseDTO { 
                    Id = session.Id, 
                    MovieId = session.MovieId, 
                    MovieTitle = movieTitle, 
                    HallId = session.HallId, 
                    HallName = hallName, 
                    StartTime = session.StartTime, 
                    Price = session.Price 
                };

                sessionDtos.Add(temp);
            }

            return new PagedList<SessionResponseDTO>( sessionDtos, pagedSessions.TotalCount, pagedSessions.CurrentPage, pagedSessions.PageSize );
        }

        private async Task<string> GetHallNameAsync(int hallId, CancellationToken cancellationToken)
        {
            var hall = await unitOfWork.Halls.GetByIdAsync(hallId, cancellationToken);
            return hall.Name;
        }

        public async Task<List<SessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var sessions = await unitOfWork.Sessions.GetAllAsync(cancellationToken);
            var orderedSessions = sessions.OrderBy(m => m.Id);
            List<SessionResponseDTO> result = new List<SessionResponseDTO>();

            foreach (var session in orderedSessions)
            {
                var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
                var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
                
                SessionResponseDTO tempSessionDTO = new SessionResponseDTO { 
                    Id = session.Id, 
                    MovieId = session.MovieId, 
                    MovieTitle = movieTitle, 
                    HallId = session.HallId, 
                    HallName = hallName, 
                    StartTime = session.StartTime, 
                    Price = session.Price 
                };
                result.Add(tempSessionDTO);
            }

            return result;
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;
            
            var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
            var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
            
            SessionResponseDTO sessionResponseDTO = new SessionResponseDTO { 
                Id = session.Id, 
                MovieId = session.MovieId, 
                MovieTitle = movieTitle, 
                HallId = session.HallId, 
                HallName = hallName, 
                StartTime = session.StartTime, 
                Price = session.Price 
            };
            return sessionResponseDTO;
        }

        public async Task<List<SessionResponseDTO>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            var sessions = await unitOfWork.Sessions.GetByMovieIdAsync(movieId, cancellationToken);
            var movieTitle = await GetMovieTitleAsync(movieId, cancellationToken);

            var orderedSessions = sessions.OrderBy(m => m.Id);
            List<SessionResponseDTO> result = new List<SessionResponseDTO>();

            foreach (var session in orderedSessions)
            {
                var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
                SessionResponseDTO sessionResponseDTO = new SessionResponseDTO { 
                    Id = session.Id, 
                    MovieId = session.MovieId, 
                    MovieTitle = movieTitle, 
                    HallId = session.HallId, 
                    HallName = hallName, 
                    StartTime = session.StartTime, 
                    Price = session.Price 
                };
                result.Add(sessionResponseDTO);
            }
            return result;
        }

        public async Task<List<SessionResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            var sessions = await unitOfWork.Sessions.GetByHallIdAsync(hallId, cancellationToken);
            var hallName = await GetHallNameAsync(hallId, cancellationToken);
            var orderedSessions = sessions.OrderBy(m => m.Id);
            var result = new List<SessionResponseDTO>();

            foreach (var session in orderedSessions)
            {
                var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
                SessionResponseDTO sessionResponseDTO = new SessionResponseDTO { 
                    Id = session.Id, 
                    MovieId = session.MovieId, 
                    MovieTitle = movieTitle, 
                    HallId = session.HallId, 
                    HallName = hallName, 
                    StartTime = session.StartTime, 
                    Price = session.Price 
                };
                result.Add(sessionResponseDTO);
            }

            return result;
        }

        public async Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            var sessions = await unitOfWork.Sessions.GetByDateRangeAsync(startDate, endDate, cancellationToken);
            var orderedSessions = sessions.OrderBy(m => m.Id);
            List<SessionResponseDTO> result = new List<SessionResponseDTO>();

            foreach (var session in orderedSessions)
            {
                var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
                var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
                SessionResponseDTO sessionResponseDTO = new SessionResponseDTO { 
                    Id = session.Id, 
                    MovieId = session.MovieId, 
                    MovieTitle = movieTitle, 
                    HallId = session.HallId, 
                    HallName = hallName, 
                    StartTime = session.StartTime, 
                    Price = session.Price 
                };

                result.Add(sessionResponseDTO);
            }
            return result;
        }

        public async Task<SessionResponseDTO> CreateAsync(SessionCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Session session = new Session { 
                MovieId = dto.MovieId, 
                HallId = dto.HallId, 
                StartTime = dto.StartTime, 
                Price = dto.Price
            };
            await unitOfWork.Sessions.CreateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
            var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
            
            SessionResponseDTO sessionResponseDTO = new SessionResponseDTO { 
                Id = session.Id, 
                MovieId = session.MovieId, 
                MovieTitle = movieTitle, 
                HallId = session.HallId, 
                HallName = hallName, 
                StartTime = session.StartTime, 
                Price = session.Price 
            };
            return sessionResponseDTO;
        }

        public async Task<SessionResponseDTO?> UpdateAsync(int id, SessionUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;

            session.StartTime = dto.StartTime;
            session.Price = dto.Price;

            unitOfWork.Sessions.Update(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            
            var movieTitle = await GetMovieTitleAsync(session.MovieId, cancellationToken);
            var hallName = await GetHallNameAsync(session.HallId, cancellationToken);
            
            SessionResponseDTO sessionResponseDTO = new SessionResponseDTO { 
                Id = session.Id, 
                MovieId = session.MovieId, 
                MovieTitle = movieTitle, 
                HallId = session.HallId, 
                HallName = hallName, 
                StartTime = session.StartTime, 
                Price = session.Price 
            };
            return sessionResponseDTO;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return false;

            unitOfWork.Sessions.Delete(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }


        public async Task<PagedList<SessionResponseDTO>> GetFilteredSessionsAsync(SessionFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Sessions.GetAll();

            if (filter.MovieId.HasValue) query = query.Where(s => s.MovieId == filter.MovieId.Value);
            if (filter.HallId.HasValue) query = query.Where(s => s.HallId == filter.HallId.Value);
            if (filter.StartTimeFrom.HasValue) query = query.Where(s => s.StartTime >= filter.StartTimeFrom.Value);
            if (filter.StartTimeTo.HasValue) query = query.Where(s => s.StartTime <= filter.StartTimeTo.Value);
            if (filter.MinPrice.HasValue) query = query.Where(s => s.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue) query = query.Where(s => s.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "starttime":
                        if (filter.SortDescending) query = query.OrderByDescending(s => s.StartTime);
                        else query = query.OrderBy(s => s.StartTime);
                        
                        break;

                    case "movieid":
                        if (filter.SortDescending) query = query.OrderByDescending(s => s.MovieId);
                        else query = query.OrderBy(s => s.MovieId);
                        
                        break;

                    case "hallid":
                        if (filter.SortDescending) query = query.OrderByDescending(s => s.HallId);
                        else query = query.OrderBy(s => s.HallId);
                        
                        break;

                    case "price":
                        if (filter.SortDescending) query = query.OrderByDescending(s => s.Price);
                        else query = query.OrderBy(s => s.Price);
                        
                        break;

                    default:
                        if (filter.SortDescending) query = query.OrderByDescending(s => s.Id);
                        else query = query.OrderBy(s => s.Id);
                        
                        break;
                }
            }
            else query = query.OrderBy(s => s.Id);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new SessionResponseDTO
                {
                    Id = s.Id,
                    MovieId = s.MovieId,
                    HallId = s.HallId,
                    StartTime = s.StartTime,
                    Price = s.Price,
                    MovieTitle = s.Movie.Title,
                    HallName = s.Hall.Name
                })
                .ToListAsync(cancellationToken);

            return new PagedList<SessionResponseDTO>(items, totalCount, pageNumber, pageSize);
        }
    }
}