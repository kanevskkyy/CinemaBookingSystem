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
        private IUnitOfWork unifOfWork;

        public GenreService(IUnitOfWork unitOfWork)
        {
            unifOfWork = unitOfWork;
        }

        public async Task<List<GenreResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var genres = await unifOfWork.Genres.GetAllAsync(cancellationToken);
            var orderedGenres = genres.OrderBy(g => g.Id);

            return orderedGenres.Select(p => new GenreResponseDTO
            {
                Id = p.Id,
                Name = p.Name
            }).ToList();
        }

        public async Task<GenreResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var genre = await unifOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) return null;
            GenreResponseDTO result = new GenreResponseDTO {
                Id = genre.Id,
                Name = genre.Name
            };

            return result;
        }

        public async Task<GenreResponseDTO> CreateAsync(GenreCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Genre genre = new Genre { 
                Name = dto.Name 
            };
            await unifOfWork.Genres.CreateAsync(genre, cancellationToken);
            await unifOfWork.SaveChangesAsync(cancellationToken);
            GenreResponseDTO result = new GenreResponseDTO { 
                Id = genre.Id, 
                Name = genre.Name 
            };

            return result;
        }

        public async Task<GenreResponseDTO?> UpdateAsync(int id, GenreUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var genre = await unifOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) return null;

            genre.Name = dto.Name;
            unifOfWork.Genres.Update(genre);
            await unifOfWork.SaveChangesAsync(cancellationToken);

            GenreResponseDTO result = new GenreResponseDTO
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return result;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var genre = await unifOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) return false;

            unifOfWork.Genres.Delete(genre);
            await unifOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        public async Task<Dictionary<string, int>> GetMovieCountsPerGenreAsync(CancellationToken cancellationToken = default)
        {
            return await unifOfWork.Genres.GetMovieCountsPerGenreAsync(cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await unifOfWork.Genres.ExistsByNameAsync(name, cancellationToken);
        }
    }
}
