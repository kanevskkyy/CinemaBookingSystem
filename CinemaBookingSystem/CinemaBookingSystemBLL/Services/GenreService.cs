using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Genres;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork _unitOfWork;

        public GenreService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<GenreResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var genres = await _unitOfWork.Genres.GetAllAsync(cancellationToken);
            return genres.Select(p => new GenreResponseDTO { Id = p.Id, Name = p.Name }).ToList();
        }

        public async Task<GenreResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) return null;

            return new GenreResponseDTO { Id = genre.Id, Name = genre.Name };
        }

        public async Task<GenreResponseDTO> CreateAsync(GenreCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Genre genre = new Genre { Name = dto.Name };
            await _unitOfWork.Genres.CreateAsync(genre, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new GenreResponseDTO { Id = genre.Id, Name = genre.Name };
        }

        public async Task<GenreResponseDTO?> UpdateAsync(int id, GenreUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) return null;

            genre.Name = dto.Name;
            _unitOfWork.Genres.Update(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new GenreResponseDTO { Id = genre.Id, Name = genre.Name };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var genre = await _unitOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) return false;

            _unitOfWork.Genres.Delete(genre);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<Dictionary<string, int>> GetMovieCountsPerGenreAsync(CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Genres.GetMovieCountsPerGenreAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _unitOfWork.Genres.ExistsByNameAsync(name, cancellationToken);
        }
    }
}
