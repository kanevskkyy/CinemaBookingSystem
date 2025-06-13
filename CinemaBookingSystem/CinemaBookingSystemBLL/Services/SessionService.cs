using AutoMapper;
using AutoMapper.QueryableExtensions;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.DTO.Sessions;
using CinemaBookingSystemBLL.Filters;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;
using Microsoft.EntityFrameworkCore;

namespace CinemaBookingSystemBLL.Services
{
    public class SessionService : ISessionService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<PagedList<SessionResponseDTO>> GetPagedSessionsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Sessions.GetAllMoviesAsyncDetail();
            PagedList<Session> pagedSessions = await PagedList<Session>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);

            List<SessionResponseDTO> sessionDtos = mapper.Map<List<SessionResponseDTO>>(pagedSessions.Items);
            return new PagedList<SessionResponseDTO>(sessionDtos, pagedSessions.TotalCount, pagedSessions.CurrentPage, pagedSessions.PageSize);
        }

        public async Task<List<SessionResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<Session> sessions = await unitOfWork.Sessions.GetAllMoviesAsyncDetail(cancellationToken).ToListAsync(cancellationToken);
            List<SessionResponseDTO> result = mapper.Map<List<SessionResponseDTO>>(sessions);
            return result;
        }

        public async Task<SessionResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Session? session = await unitOfWork.Sessions.GetByIdWithDetailsAsync(id, cancellationToken);
            if (session == null) return null;
            return mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<List<SessionResponseDTO>> GetByMovieIdAsync(Guid movieId, CancellationToken cancellationToken = default)
        {
            List<Session> sessions = await unitOfWork.Sessions.GetByMovieIdAsync(movieId, cancellationToken);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default)
        {
            List<Session> sessions = await unitOfWork.Sessions.GetByHallIdAsync(hallId, cancellationToken);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            List<Session> sessions = await unitOfWork.Sessions.GetByDateRangeAsync(startDate, endDate, cancellationToken);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<SessionResponseDTO> CreateAsync(SessionCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Movie? movie = await unitOfWork.Movies.GetByIdAsync(dto.MovieId, cancellationToken);
            if (movie == null) throw new ArgumentException("Movie not found.");

            var sessionsInHall = await unitOfWork.Sessions.GetAllAsync();
            
            DateTime newStart = dto.StartTime.ToUniversalTime();
            DateTime newEnd = newStart.AddMinutes(movie.Duration).ToUniversalTime();
            foreach (var existingSession in sessionsInHall)
            {
                Movie existingMovie = await unitOfWork.Movies.GetByIdAsync(existingSession.MovieId, cancellationToken);
                
                DateTime existingStart = existingSession.StartTime.ToUniversalTime();
                DateTime existingEnd = existingStart.AddMinutes(existingMovie.Duration).ToUniversalTime();
                
                if (newStart < existingEnd && existingStart < newEnd) throw new InvalidOperationException("This hall already has a session during this time.");
            }

            Session session = mapper.Map<Session>(dto);
            await unitOfWork.Sessions.CreateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            Session createdSession = await unitOfWork.Sessions.GetByIdWithDetailsAsync(session.Id, cancellationToken);
            return mapper.Map<SessionResponseDTO>(createdSession);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Session session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return false;

            unitOfWork.Sessions.Delete(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<SessionResponseDTO?> UpdateAsync(Guid id, SessionUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Session session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;

            Movie movie = await unitOfWork.Movies.GetByIdAsync(session.MovieId, cancellationToken);
            if (movie == null) throw new ArgumentException("Movie not found.");

            DateTime newStart = dto.StartTime.ToUniversalTime();
            DateTime newEnd = newStart.AddMinutes(movie.Duration).ToUniversalTime();
            var sessionsInHall = await unitOfWork.Sessions.FindAsync(s => s.HallId == session.HallId && s.Id != id, cancellationToken);

            foreach (var existingSession in sessionsInHall)
            {
                Movie existingMovie = await unitOfWork.Movies.GetByIdAsync(existingSession.MovieId, cancellationToken);
                DateTime existingStart = existingSession.StartTime.ToUniversalTime();
                DateTime existingEnd = existingStart.AddMinutes(existingMovie.Duration).ToUniversalTime();

                if (newStart < existingEnd && existingStart < newEnd) throw new InvalidOperationException("This hall already has a session during this time.");
            }

            session.StartTime = dto.StartTime;
            session.Price = dto.Price;
            unitOfWork.Sessions.Update(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            Session updatedSession = await unitOfWork.Sessions.GetByIdWithDetailsAsync(id, cancellationToken);
            return mapper.Map<SessionResponseDTO>(updatedSession);
        }

        public async Task<PagedList<SessionResponseDTO>> GetFilteredSessionsAsync(SessionFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Session> queryable = unitOfWork.Sessions.GetAll();

            if (filter.MovieId.HasValue) queryable = queryable.Where(s => s.MovieId == filter.MovieId.Value);
            if (filter.HallId.HasValue) queryable = queryable.Where(s => s.HallId == filter.HallId.Value);
            if (filter.StartTimeFrom.HasValue) queryable = queryable.Where(s => s.StartTime >= filter.StartTimeFrom.Value);
            if (filter.StartTimeTo.HasValue) queryable = queryable.Where(s => s.StartTime <= filter.StartTimeTo.Value);
            if (filter.MinPrice.HasValue) queryable = queryable.Where(s => s.Price >= filter.MinPrice.Value);
            if (filter.MaxPrice.HasValue) queryable = queryable.Where(s => s.Price <= filter.MaxPrice.Value);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "starttime":
                        queryable = filter.SortDescending ? queryable.OrderByDescending(s => s.StartTime) : queryable.OrderBy(s => s.StartTime);
                        break;
                    case "movieid":
                        queryable = filter.SortDescending ? queryable.OrderByDescending(s => s.MovieId) : queryable.OrderBy(s => s.MovieId);
                        break;
                    case "hallid":
                        queryable = filter.SortDescending ? queryable.OrderByDescending(s => s.HallId) : queryable.OrderBy(s => s.HallId);
                        break;
                    case "price":
                        queryable = filter.SortDescending ? queryable.OrderByDescending(s => s.Price) : queryable.OrderBy(s => s.Price);
                        break;
                    default:
                        queryable = filter.SortDescending ? queryable.OrderByDescending(s => s.Id) : queryable.OrderBy(s => s.Id);
                        break;
                }
            }
            else queryable = queryable.OrderBy(s => s.Id);

            var projectedQuery = queryable.ProjectTo<SessionResponseDTO>(mapper.ConfigurationProvider);/**/
            var pagedList = await PagedList<SessionResponseDTO>.ToPagedListAsync(projectedQuery, pageNumber, pageSize, cancellationToken);
            
            return pagedList;
        }
    }
}  