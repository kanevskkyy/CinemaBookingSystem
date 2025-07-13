using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Genres;
using CinemaBookingSystemBLL.Exceptions;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class GenreService : IGenreService
    {
        private IMapper mapper;
        private IUnitOfWork unitOfWork;

        public GenreService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<GenreResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            List<Genre> genres = await unitOfWork.Genres.GetAllAsync(cancellationToken);
            List<Genre> orderedGenres = genres.OrderBy(p => p.Name).ToList();

            return mapper.Map<List<GenreResponseDTO>>(orderedGenres);
        }

        public async Task<GenreResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Genre? genre = await unitOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) throw new NotFoundException("Genre", id);
            
            return mapper.Map<GenreResponseDTO>(genre);
        }

        public async Task<GenreResponseDTO> CreateAsync(GenreCreateDTO dto, CancellationToken cancellationToken = default)
        {
            bool existByGenre = await unitOfWork.Genres.ExistsByNameAsync(dto.Name, null, cancellationToken);
            if (existByGenre) throw new EntityAlreadyExistsException("Genre", "Name", dto.Name);

            Genre genre = mapper.Map<Genre>(dto);

            await unitOfWork.Genres.CreateAsync(genre, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<GenreResponseDTO>(genre);
        }

        public async Task<GenreResponseDTO?> UpdateAsync(Guid id, GenreUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Genre? genre = await unitOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) throw new NotFoundException("Genre", id);

            bool existByGenre = await unitOfWork.Genres.ExistsByNameAsync(dto.Name, id, cancellationToken);
            if (existByGenre) throw new EntityAlreadyExistsException("Genre", "Name", dto.Name);

            mapper.Map(dto, genre);

            unitOfWork.Genres.Update(genre);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return mapper.Map<GenreResponseDTO>(genre);
        }


        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Genre? genre = await unitOfWork.Genres.GetByIdAsync(id, cancellationToken);
            if (genre == null) throw new NotFoundException("Genre", id);

            unitOfWork.Genres.Delete(genre);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<Dictionary<string, int>> GetMovieCountsPerGenreAsync(CancellationToken cancellationToken = default)
        {
            return await unitOfWork.Genres.GetMovieCountsPerGenreAsync(cancellationToken);
        }
    }
}
