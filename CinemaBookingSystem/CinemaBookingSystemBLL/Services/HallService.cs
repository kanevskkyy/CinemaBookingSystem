using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Halls;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class HallService : IHallService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public HallService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<List<HallResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var halls = await unitOfWork.Halls.GetAllAsync(cancellationToken);
            var orderedHalls = halls.OrderBy(h => h.Id);

            return mapper.Map<List<HallResponseDTO>>(orderedHalls);
        }

        public async Task<HallResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);

            if (hall == null) return null;
            else return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<HallResponseDTO?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            Hall? hall = await unitOfWork.Halls.GetByNameAsync(name, cancellationToken);

            if (hall == null) return null;
            else return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<HallResponseDTO> CreateAsync(HallCreateDTO dto, CancellationToken cancellationToken = default)
        {
            var existsHall = await unitOfWork.Halls.FindAsync(p => p.Name.ToLower() == dto.Name.ToLower(), cancellationToken);
            if (existsHall.Any()) throw new ArgumentException("Hall with this name already exists");

            Hall hall = mapper.Map<Hall>(dto);
            await unitOfWork.Halls.CreateAsync(hall, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<HallResponseDTO?> UpdateAsync(Guid id, HallUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) return null;

            var existingHalls = await unitOfWork.Halls.FindAsync(h => h.Name.ToLower() == dto.Name.ToLower() && h.Id != id, cancellationToken);
            if (existingHalls.Any()) throw new ArgumentException("Hall with this name already exists");

            mapper.Map(dto, hall);

            unitOfWork.Halls.Update(hall);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) return false;

            unitOfWork.Halls.Delete(hall);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
