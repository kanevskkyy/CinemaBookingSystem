using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Halls;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class HallService : IHallService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HallService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<HallResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var halls = await _unitOfWork.Halls.GetAllAsync(cancellationToken);
            return halls.Select(p => new HallResponseDTO { Id = p.Id, Name = p.Name, RowsAmount = p.RowsAmount, SeatsPerRow = p.SeatsPerRow }).ToList();
        }

        public async Task<HallResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var hall = await _unitOfWork.Halls.GetByIdAsync(id, cancellationToken);

            if (hall == null) return null;
            else return new HallResponseDTO{ Id = hall.Id, Name = hall.Name, RowsAmount = hall.RowsAmount, SeatsPerRow = hall.SeatsPerRow };
        }

        public async Task<HallResponseDTO?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var hall = await _unitOfWork.Halls.GetByNameAsync(name, cancellationToken);
            
            if (hall == null) return null;
            else return new HallResponseDTO { Id = hall.Id, Name = hall.Name, RowsAmount = hall.RowsAmount, SeatsPerRow = hall.SeatsPerRow };
        }

        public async Task<HallResponseDTO> CreateAsync(HallCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Hall hall = new Hall { Name = dto.Name, RowsAmount = dto.RowAmount, SeatsPerRow = dto.SeatsPerRow };
            await _unitOfWork.Halls.CreateAsync(hall, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new HallResponseDTO { Id = hall.Id, Name = hall.Name, RowsAmount = hall.RowsAmount, SeatsPerRow = hall.SeatsPerRow };
        }

        public async Task<HallResponseDTO?> UpdateAsync(int id, HallUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var hall = await _unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) return null;

            hall.Name = dto.Name;
            hall.RowsAmount = dto.RowsAmount;
            hall.SeatsPerRow = dto.SeatsPerRow;

            _unitOfWork.Halls.Update(hall);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new HallResponseDTO { Id = hall.Id, Name = hall.Name, RowsAmount = hall.RowsAmount, SeatsPerRow = hall.SeatsPerRow };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var hall = await _unitOfWork.Halls.GetByIdAsync(id, cancellationToken);
            if (hall == null) return false;

            _unitOfWork.Halls.Delete(hall);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
