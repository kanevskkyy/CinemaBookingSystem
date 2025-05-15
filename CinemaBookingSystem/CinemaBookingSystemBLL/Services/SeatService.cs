using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Pagination;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class SeatService : ISeatService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SeatService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<SeatResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var seats = await _unitOfWork.Seats.GetAllAsync(cancellationToken);
            return seats.Select(p => new SeatResponseDTO { Id = p.Id, HallId = p.HallId, RowNumber = p.RowNumber, SeatNumber = p.SeatNumber}).ToList();
        }

        public async Task<PagedList<SeatResponseDTO>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var paginated = await _unitOfWork.Seats.GetPagedSeatsAsync(pageNumber, pageSize, cancellationToken);
            var seats = paginated.Select(seat => new SeatResponseDTO { Id = seat.Id, HallId = seat.HallId, RowNumber = seat.RowNumber, SeatNumber = seat.SeatNumber}).ToList();
            return new PagedList<SeatResponseDTO>(seats, paginated.TotalCount, paginated.CurrentPage, paginated.PageSize);
        }


        public async Task<SeatResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var seat = await _unitOfWork.Seats.GetByIdAsync(id, cancellationToken);

            if (seat == null) return null;
            else return new SeatResponseDTO { Id = seat.Id, HallId = seat.HallId, RowNumber = seat.RowNumber, SeatNumber = seat.SeatNumber };
        }

        public async Task<List<SeatResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            var seats = await _unitOfWork.Seats.GetByHallIdAsync(hallId, cancellationToken);
            return seats.Select(p => new SeatResponseDTO { Id = p.Id, HallId = p.HallId, RowNumber = p.RowNumber, SeatNumber = p.SeatNumber }).ToList();
        }

        public async Task<SeatResponseDTO?> GetByRowAndNumberAsync(int hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken = default)
        {
            var seat = await _unitOfWork.Seats.GetByRowAndNumberAsync(hallId, rowNumber, seatNumber, cancellationToken);
            
            if (seat == null) return null;
            else return new SeatResponseDTO { Id = seat.Id, HallId = seat.HallId, RowNumber = seat.RowNumber, SeatNumber = seat.SeatNumber };
        }

        public async Task<SeatResponseDTO> CreateAsync(SeatCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Seat seat = new Seat { HallId = dto.HallId, RowNumber = dto.RowNumber, SeatNumber = dto.SeatNumber };
            await _unitOfWork.Seats.CreateAsync(seat, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SeatResponseDTO { Id = seat.Id, HallId = seat.HallId, RowNumber = seat.RowNumber, SeatNumber = seat.SeatNumber };
        }

        public async Task<SeatResponseDTO?> UpdateAsync(int id, SeatUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            var seat = await _unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return null;

            seat.RowNumber = dto.RowNumber;
            seat.SeatNumber = dto.SeatNumber;

            _unitOfWork.Seats.Update(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new SeatResponseDTO { Id = seat.Id, HallId = seat.HallId, RowNumber = seat.RowNumber, SeatNumber = seat.SeatNumber };
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var seat = await _unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return false;

            _unitOfWork.Seats.Delete(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
