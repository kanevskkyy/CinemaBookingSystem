using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class MovieService : IMovieService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MovieService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<MovieResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var movies = await _unitOfWork.Movies.GetAllAsync(cancellationToken);
            return movies.Select(movie => new MovieResponseDTO { Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating }).ToList();
        }

        public async Task<MovieResponseDTO> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id, cancellationToken);

            if (movie == null) return null;
            else return new MovieResponseDTO{ Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating };
        }

        public async Task<List<MovieResponseDTO>> GetByGenreAsync(int genreId, CancellationToken cancellationToken = default)
        {
            var movies = await _unitOfWork.Movies.GetByGenreAsync(genreId, cancellationToken);
            return movies.Select(movie => new MovieResponseDTO { Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating }).ToList();
        }

        public async Task<List<MovieResponseDTO>> GetTopRatedAsync(CancellationToken cancellationToken = default)
        {
            var movies = await _unitOfWork.Movies.GetTopRatedAsync(cancellationToken);
            return movies.Select(movie => new MovieResponseDTO{ Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating }).ToList();
        }

        public async Task<MovieResponseDTO> CreateAsync(MovieCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Movie movie = new Movie{ Title = dto.Title, Description = dto.Description, Duration = dto.Duration, PosterUrl = dto.PosterUrl, GenreId = dto.GenreId, Rating = dto.Rating };

            await _unitOfWork.Movies.CreateAsync(movie, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new MovieResponseDTO { Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating };
        }

        public async Task<MovieResponseDTO> UpdateAsync(int id, MovieUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return null;

            movie.Title = dto.Title;
            movie.Description = dto.Description;
            movie.Duration = dto.Duration;
            movie.PosterUrl = dto.PosterUrl;
            movie.GenreId = dto.GenreId;
            movie.Rating = dto.Rating;

            _unitOfWork.Movies.Update(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new MovieResponseDTO { Id = movie.Id, Title = movie.Title, Description = movie.Description, Duration = movie.Duration, PosterUrl = movie.PosterUrl, GenreId = movie.GenreId, Rating = movie.Rating };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var movie = await _unitOfWork.Movies.GetByIdAsync(id, cancellationToken);
            if (movie == null) return false;

            _unitOfWork.Movies.Delete(movie);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}