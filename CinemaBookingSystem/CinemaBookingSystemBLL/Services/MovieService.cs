using Microsoft.EntityFrameworkCore;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemBLL.Filters;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Runtime.CompilerServices;

namespace CinemaBookingSystemBLL.Services
{
    public class MovieService : IMovieService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;
        public MovieService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<MovieResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var movies = await unitOfWork.Movies.GetAllAsync(cancellationToken);
            var orderedMovies = movies.OrderBy(m => m.Id);

            return mapper.Map<List<MovieResponseDTO>>(orderedMovies);
        }

        public async Task<MovieResponseDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Movie movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);

            if (movie == null) return null;
            else return mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<List<MovieResponseDTO>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default)
        {
            List<Movie> movies = await unitOfWork.Movies.GetByGenreAsync(genreId, cancellationToken);

            return mapper.Map<List<MovieResponseDTO>>(movies);
        }

        public async Task<List<MovieResponseDTO>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            List<Movie> movies = await unitOfWork.Movies.GetTopRatedAsync(cancellationToken);
            return mapper.Map<List<MovieResponseDTO>>(movies);
        }

        public async Task<MovieResponseDTO> CreateAsync(MovieCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var existsMovie = await unitOfWork.Movies.FindAsync(p => p.Title.ToLower() == dto.Title.ToLower(), cancellationToken);
            if (existsMovie.Any()) throw new ArgumentException("Movie with this title already exists");

            Movie movie = mapper.Map<Movie>(dto);

            await unitOfWork.Movies.CreateAsync(movie, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<MovieResponseDTO> UpdateAsync(int id, MovieUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Movie movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return null;

            var existsMovie = await unitOfWork.Movies.FindAsync(p => p.Id != id && p.Title.ToLower() == movie.Title.ToLower(), cancellationToken);
            if (existsMovie.Any()) throw new ArgumentException("Film with this title already exists!");

            mapper.Map(dto, movie);

            unitOfWork.Movies.Update(movie);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Movie movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return false;

            unitOfWork.Movies.Delete(movie);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PagedList<MovieResponseDTO>> GetPagedMoviesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Movies.GetAll();
            PagedList<Movie> pagedMovies = await PagedList<Movie>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            List<MovieResponseDTO> movieDtos = mapper.Map<List<MovieResponseDTO>>(pagedMovies.Items);

            return new PagedList<MovieResponseDTO>(movieDtos, pagedMovies.TotalCount, pagedMovies.CurrentPage, pagedMovies.PageSize);
        }

        public async Task<PagedList<MovieResponseDTO>> GetFilteredMoviesAsync(MovieFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Movies.GetAll();

            if (!string.IsNullOrEmpty(filter.Title)) query = query.Where(m => m.Title.ToLower().Contains(filter.Title.ToLower()));
            if (filter.GenreId.HasValue) query = query.Where(m => m.GenreId == filter.GenreId.Value);
            if (filter.MinRating.HasValue) query = query.Where(m => m.Rating >= filter.MinRating.Value);
            if (filter.MaxRating.HasValue) query = query.Where(m => m.Rating <= filter.MaxRating.Value);
            if (filter.MinDuration.HasValue) query = query.Where(m => m.Duration >= filter.MinDuration.Value);
            if (filter.MaxDuration.HasValue) query = query.Where(m => m.Duration <= filter.MaxDuration.Value);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "title":
                        if (filter.SortDescending) query = query.OrderByDescending(m => m.Title);
                        else query = query.OrderBy(m => m.Title);

                        break;

                    case "rating":
                        if (filter.SortDescending) query = query.OrderByDescending(m => m.Rating);
                        else query = query.OrderBy(m => m.Rating);

                        break;

                    case "duration":
                        if (filter.SortDescending) query = query.OrderByDescending(m => m.Duration);
                        else query = query.OrderBy(m => m.Duration);

                        break;

                    case "genreid":
                        if (filter.SortDescending) query = query.OrderByDescending(m => m.GenreId);
                        else query = query.OrderBy(m => m.GenreId);

                        break;

                    default:
                        if (filter.SortDescending) query = query.OrderByDescending(m => m.Id);
                        else query = query.OrderBy(m => m.Id);

                        break;
                }
            }
            else query = query.OrderBy(m => m.Id);

            var dtoQuery = query.ProjectTo<MovieResponseDTO>(mapper.ConfigurationProvider);

            var pagedResult = await PagedList<MovieResponseDTO>.ToPagedListAsync(dtoQuery, pageNumber, pageSize, cancellationToken);
            return pagedResult;
        }
    }
}