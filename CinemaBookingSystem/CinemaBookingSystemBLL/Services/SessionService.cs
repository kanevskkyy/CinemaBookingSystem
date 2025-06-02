using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<SessionResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Session session = await unitOfWork.Sessions.GetByIdWithDetailsAsync(id, cancellationToken);
            if (session == null) return null;
            return mapper.Map<SessionResponseDTO>(session);
        }

        public async Task<List<SessionResponseDTO>> GetByMovieIdAsync(int movieId, CancellationToken cancellationToken = default)
        {
            List<Session> sessions = await unitOfWork.Sessions.GetByMovieIdAsync(movieId, cancellationToken);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            var sessions = await unitOfWork.Sessions.GetByHallIdAsync(hallId, cancellationToken);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<List<SessionResponseDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            List<Session> sessions = await unitOfWork.Sessions.GetByDateRangeAsync(startDate, endDate, cancellationToken);
            return mapper.Map<List<SessionResponseDTO>>(sessions);
        }

        public async Task<SessionResponseDTO> CreateAsync(SessionCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Session session = mapper.Map<Session>(dto);

            await unitOfWork.Sessions.CreateAsync(session, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            Session createdSession = await unitOfWork.Sessions.GetByIdWithDetailsAsync(session.Id, cancellationToken);
            return mapper.Map<SessionResponseDTO>(createdSession);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Session session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return false;

            unitOfWork.Sessions.Delete(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<SessionResponseDTO?> UpdateAsync(int id, SessionUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Session session = await unitOfWork.Sessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;

            mapper.Map(dto, session);

            unitOfWork.Sessions.Update(session);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            Session updatedSession = await unitOfWork.Sessions.GetByIdWithDetailsAsync(id, cancellationToken);
            return mapper.Map<SessionResponseDTO>(updatedSession);
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
                        query = filter.SortDescending ? query.OrderByDescending(s => s.StartTime) : query.OrderBy(s => s.StartTime);
                        break;
                    case "movieid":
                        query = filter.SortDescending ? query.OrderByDescending(s => s.MovieId) : query.OrderBy(s => s.MovieId);
                        break;
                    case "hallid":
                        query = filter.SortDescending ? query.OrderByDescending(s => s.HallId) : query.OrderBy(s => s.HallId);
                        break;
                    case "price":
                        query = filter.SortDescending ? query.OrderByDescending(s => s.Price) : query.OrderBy(s => s.Price);
                        break;
                    default:
                        query = filter.SortDescending ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id);
                        break;
                }
            }
            else query = query.OrderBy(s => s.Id);

            var projectedQuery = query.ProjectTo<SessionResponseDTO>(mapper.ConfigurationProvider);/**/
            var pagedList = await PagedList<SessionResponseDTO>.ToPagedListAsync(projectedQuery, pageNumber, pageSize, cancellationToken);
            
            return pagedList;
        }
    }
}  