using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Halls;
using CinemaBookingSystemBLL.Exceptions;
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
            List<Hall> halls = await unitOfWork.Halls.GetAllAsync(cancellationToken);
            List<Hall> orderedHalls = halls.OrderBy(h =>
            {
                string name = h.Name;
                int index = -1;
                
                for (int i = 0; i < name.Length; i++)
                {
                    if (char.IsDigit(name[i]))
                    {
                        index = i;
                        break;
                    }
                }
                
                if (index == -1) return int.MaxValue;
                string number = name.Substring(index);
              
                if (int.TryParse(number, out int result)) return result;
                else return int.MaxValue;
            })
            .ThenBy(h => h.Name)
            .ToList();   

            return mapper.Map<List<HallResponseDTO>>(orderedHalls);
        }

        public async Task<HallResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) throw new NotFoundException("Hall", id);
            else return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<HallResponseDTO> CreateAsync(HallCreateDTO dto, CancellationToken cancellationToken = default)
        {
            bool existsByName = await unitOfWork.Halls.ExistsByNameAsync(dto.Name, cancellationToken: cancellationToken);
            if (existsByName) throw new EntityAlreadyExistsException("Hall", "Name", dto.Name);

            Hall hall = mapper.Map<Hall>(dto);
            await unitOfWork.Halls.CreateAsync(hall, cancellationToken);

            for (int i = 1; i <= dto.RowsAmount; i++)
            {
                for (int seatNumber = 1; seatNumber <= dto.SeatsPerRow; seatNumber++)
                {
                    Seat seat = new Seat
                    {
                        Id = Guid.NewGuid(),
                        HallId = hall.Id,
                        RowNumber = i,
                        SeatNumber = seatNumber
                    };
                    await unitOfWork.Seats.CreateAsync(seat, cancellationToken);
                }
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<HallResponseDTO?> UpdateAsync(Guid id, HallUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) throw new NotFoundException("Hall", id);

            bool exist = await unitOfWork.Halls.ExistsByNameAsync(dto.Name, id, cancellationToken);
            if (exist) throw new EntityAlreadyExistsException("Hall", "Name", dto.Name);

            mapper.Map(dto, hall);

            unitOfWork.Halls.Update(hall);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) throw new NotFoundException("Hall", id);

            unitOfWork.Halls.Delete(hall);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}