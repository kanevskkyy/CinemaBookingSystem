using Microsoft.EntityFrameworkCore;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class MovieService : IMovieService
    {
        private IUnitOfWork unitOfWork;

        public MovieService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<List<MovieResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var movies = await unitOfWork.Movies.GetAllAsync(cancellationToken);
            var orderedMovies = movies.OrderBy(m => m.Id);

            return orderedMovies.Select(movie => new MovieResponseDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Duration = movie.Duration,
                PosterUrl = movie.PosterUrl,
                GenreId = movie.GenreId,
                Rating = movie.Rating
            }).ToList();
        }

        public async Task<MovieResponseDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);

            if (movie == null) return null;
            else
            {
                MovieResponseDTO result = new MovieResponseDTO { 
                    Id = movie.Id, 
                    Title = movie.Title, 
                    Description = movie.Description, 
                    Duration = movie.Duration, 
                    PosterUrl = movie.PosterUrl, 
                    GenreId = movie.GenreId, 
                    Rating = movie.Rating 
                };
                return result;
            }
        }

        public async Task<List<MovieResponseDTO>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default)
        {
            var movies = await unitOfWork.Movies.GetByGenreAsync(genreId, cancellationToken);
            
            return movies.Select(movie => new MovieResponseDTO { 
                Id = movie.Id, 
                Title = movie.Title, 
                Description = movie.Description, 
                Duration = movie.Duration, 
                PosterUrl = movie.PosterUrl, 
                GenreId = movie.GenreId, 
                Rating = movie.Rating 
            }).ToList();
        }

        public async Task<List<MovieResponseDTO>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            var movies = await unitOfWork.Movies.GetTopRatedAsync(cancellationToken);

            var orderedMovies = movies.OrderBy(m => m.Id);
            return orderedMovies.Select(movie => new MovieResponseDTO{ 
                Id = movie.Id, 
                Title = movie.Title, 
                Description = movie.Description, 
                Duration = movie.Duration, 
                PosterUrl = movie.PosterUrl, 
                GenreId = movie.GenreId, 
                Rating = movie.Rating 
            }).ToList();
        }

        public async Task<MovieResponseDTO> CreateAsync(MovieCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Movie movie = new Movie{ 
                Title = dto.Title, 
                Description = dto.Description, 
                Duration = dto.Duration, 
                PosterUrl = dto.PosterUrl, 
                GenreId = dto.GenreId, 
                Rating = dto.Rating 
            };

            await unitOfWork.Movies.CreateAsync(movie, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            MovieResponseDTO result = new MovieResponseDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Duration = movie.Duration,
                PosterUrl = movie.PosterUrl,
                GenreId = movie.GenreId,
                Rating = movie.Rating
            };

            return result;
        }

        public async Task<MovieResponseDTO> UpdateAsync(int id, MovieUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return null;

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.Duration = dto.Duration;
            movie.PosterUrl = dto.PosterUrl;
            movie.GenreId = dto.GenreId;
            movie.Rating = dto.Rating;

            unitOfWork.Movies.Update(movie);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            MovieResponseDTO result = new MovieResponseDTO
            {
                Id = movie.Id,
                Title = movie.Title,
                Description = movie.Description,
                Duration = movie.Duration,
                PosterUrl = movie.PosterUrl,
                GenreId = movie.GenreId,
                Rating = movie.Rating
            };

            return result;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var movie = await unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return false;

            unitOfWork.Movies.Delete(movie);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<PagedList<MovieResponseDTO>> GetPagedMoviesAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var pagedMovies = await unitOfWork.Movies.GetPagedMoviesAsync(pageNumber, pageSize, cancellationToken);
            var movieDtos = pagedMovies.Select(movie => new MovieResponseDTO { Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating }).ToList();
            
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

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(movie => new MovieResponseDTO
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Description = movie.Description,
                    Duration = movie.Duration,
                    PosterUrl = movie.PosterUrl,
                    GenreId = movie.GenreId,
                    Rating = movie.Rating
                })
                .ToListAsync(cancellationToken);

            return new PagedList<MovieResponseDTO>(items, totalCount, pageNumber, pageSize);
        }

    }
}