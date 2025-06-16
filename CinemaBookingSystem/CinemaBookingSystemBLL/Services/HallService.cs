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
            var orderedHalls = halls.OrderBy(h =>
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
            .ThenBy(h => h.Name);   

            return mapper.Map<List<HallResponseDTO>>(orderedHalls);
        }

        public async Task<HallResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Hall hall = await unitOfWork.Halls.GetByIdAsync(id, cancellationToken);

            if (hall == null) return null;
            else return mapper.Map<HallResponseDTO>(hall);
        }

        public async Task<HallResponseDTO> CreateAsync(HallCreateDTO dto, CancellationToken cancellationToken = default)
        {
            bool exists = await unitOfWork.Halls.ExistsByNameAsync(dto.Name, cancellationToken: cancellationToken);
            if (exists) throw new ArgumentException("Hall with this name already exists");

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
            if (hall == null) return null;

            bool exist = await unitOfWork.Halls.ExistsByNameAsync(dto.Name, id, cancellationToken);
            if (exist) throw new ArgumentException("Hall with this name already exists");

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
