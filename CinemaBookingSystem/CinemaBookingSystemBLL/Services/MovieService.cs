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
            var movies = await unitOfWork.Movies.GetAllWithGenresAsync(cancellationToken);
            var orderedMovies = movies.OrderBy(m => m.Id);

            return mapper.Map<List<MovieResponseDTO>>(orderedMovies);
        }

        public async Task<MovieResponseDTO> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Movie? movie = await unitOfWork.Movies.GetByIdWithGenresAsync(id, cancellationToken);

            if (movie == null) return null;
            else return mapper.Map<MovieResponseDTO>(movie);
        }

        public async Task<List<MovieResponseDTO>> GetByGenreAsync(Guid genreId, CancellationToken cancellationToken = default)
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
            var existByTitle = await unitOfWork.Movies.FindAsync(p => p.Title.ToLower() == dto.Title.ToLower(), cancellationToken);
            if (existByTitle.Any()) throw new ArgumentException("Movie with this title already exists");

            var existByPoster = await unitOfWork.Movies.FindAsync(p => p.PosterUrl.ToLower() == dto.PosterUrl.ToLower(), cancellationToken);
            if (existByPoster.Any()) throw new ArgumentException("Movie with this poster URL already exists");

            Movie movie = mapper.Map<Movie>(dto);

            await unitOfWork.Movies.CreateAsync(movie, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            Movie? result = await unitOfWork.Movies.GetByIdWithGenresAsync(movie.Id, cancellationToken);

            return mapper.Map<MovieResponseDTO>(result);
        }

        public async Task<MovieResponseDTO> UpdateAsync(Guid id, MovieUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Movie movie = await unitOfWork.Movies.GetByIdWithGenresAsync(id, cancellationToken);
            if (movie == null) return null;

            var existByTitle = await unitOfWork.Movies.FindAsync(p => p.Id != id && p.Title.ToLower() == dto.Title.ToLower(), cancellationToken);
            if (existByTitle.Any()) throw new ArgumentException("Movie with this title already exists");

            var existByPoster = await unitOfWork.Movies.FindAsync(p => p.Id != id && p.PosterUrl.ToLower() == dto.PosterUrl.ToLower(), cancellationToken);
            if (existByPoster.Any()) throw new ArgumentException("Movie with this poster URL already exists");

            mapper.Map(dto, movie);
            movie.MovieGenres.Clear();
            await unitOfWork.SaveChangesAsync(cancellationToken);

            foreach (Guid genreId in dto.GenreIds)
            {
                MovieGenre movieGenre = new MovieGenre
                {
                    MovieId = movie.Id,
                    GenreId = genreId
                };

                movie.MovieGenres.Add(movieGenre);
            }

            unitOfWork.Movies.Update(movie);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<MovieResponseDTO>(movie);
        }


        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Movie movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return false;

            unitOfWork.Movies.Delete(movie);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PagedList<MovieResponseDTO>> GetPagedMoviesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var queryable = unitOfWork.Movies.GetAll();
            
            PagedList<Movie> pagedMovies = await PagedList<Movie>.ToPagedListAsync(queryable, pageNumber, pageSize, cancellationToken);
            List<MovieResponseDTO> movieDtos = mapper.Map<List<MovieResponseDTO>>(pagedMovies.Items);
            
            return new PagedList<MovieResponseDTO>(movieDtos, pagedMovies.TotalCount, pagedMovies.CurrentPage, pagedMovies.PageSize);
        }

        public async Task<PagedList<MovieResponseDTO>> GetFilteredMoviesAsync(MovieFilterDTO filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Movie> queryable = unitOfWork.Movies.GetAll().AsQueryable();

            if (!string.IsNullOrEmpty(filter.Title)) queryable = queryable.Where(m => m.Title.ToLower().Contains(filter.Title.ToLower()));
            if (filter.GenreId.HasValue) queryable = queryable.Where(m => m.MovieGenres.Any(mg => mg.GenreId == filter.GenreId.Value));
            if (filter.MinRating.HasValue) queryable = queryable.Where(m => m.Rating >= filter.MinRating.Value);
            if (filter.MaxRating.HasValue) queryable = queryable.Where(m => m.Rating <= filter.MaxRating.Value);
            if (filter.MinDuration.HasValue) queryable = queryable.Where(m => m.Duration >= filter.MinDuration.Value);
            if (filter.MaxDuration.HasValue) queryable = queryable.Where(m => m.Duration <= filter.MaxDuration.Value);

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "title":
                        if (filter.SortDescending)
                        {
                            queryable = queryable.OrderByDescending(m => m.Title);
                        }
                        else
                        {
                            queryable = queryable.OrderBy(m => m.Title);
                        }
                        break;

                    case "rating":
                        if (filter.SortDescending)
                        {
                            queryable = queryable.OrderByDescending(m => m.Rating);
                        }
                        else
                        {
                            queryable = queryable.OrderBy(m => m.Rating);
                        }
                        break;

                    case "duration":
                        if (filter.SortDescending)
                        {
                            queryable = queryable.OrderByDescending(m => m.Duration);
                        }
                        else
                        {
                            queryable = queryable.OrderBy(m => m.Duration);
                        }
                        break;

                    default:
                        if (filter.SortDescending)
                        {
                            queryable = queryable.OrderByDescending(m => m.Id);
                        }
                        else
                        {
                            queryable = queryable.OrderBy(m => m.Id);
                        }
                        break;
                }
            }
            else
            {
                queryable = queryable.OrderBy(m => m.Id);
            }

            queryable = queryable.Include(m => m.MovieGenres);
            var resltDTO = queryable.ProjectTo<MovieResponseDTO>(mapper.ConfigurationProvider);
            var pagedResult = await PagedList<MovieResponseDTO>.ToPagedListAsync(resltDTO, pageNumber, pageSize, cancellationToken);
            return pagedResult;
        }

    }
}