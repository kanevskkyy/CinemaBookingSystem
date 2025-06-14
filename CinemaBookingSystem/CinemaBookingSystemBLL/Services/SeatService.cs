using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CinemaBookingSystemBLL.DTO.Seats;
using CinemaBookingSystemBLL.Interfaces;
using CinemaBookingSystemBLL.Pagination;
using CinemaBookingSystemDAL.Entities;
using CinemaBookingSystemDAL.Unit_of_Work;

namespace CinemaBookingSystemBLL.Services
{
    public class SeatService : ISeatService
    {
        private IUnitOfWork unitOfWork;
        private IMapper mapper;

        public SeatService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<PagedList<SeatResponseDTO>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            IQueryable<Seat> query = unitOfWork.Seats.GetAll();
            PagedList<Seat> pagedSeats = await PagedList<Seat>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            List<SeatResponseDTO> seatDtos = mapper.Map<List<SeatResponseDTO>>(pagedSeats.Items);

            return new PagedList<SeatResponseDTO>(seatDtos, pagedSeats.TotalCount, pagedSeats.CurrentPage, pagedSeats.PageSize);
        }

        public async Task<SeatResponseDTO?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Seat seat = await unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return null;

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<List<SeatResponseDTO>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default)
        {
            List<Seat> seats = await unitOfWork.Seats.GetByHallIdAsync(hallId, cancellationToken);
            return mapper.Map<List<SeatResponseDTO>>(seats);
        }

        public async Task<SeatResponseDTO> CreateAsync(SeatCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Hall? hall = await unitOfWork.Halls.GetByIdAsync(dto.HallId, cancellationToken);

            if (hall == null) throw new ArgumentException("Hall with the specified ID does not exist");
            if (dto.SeatNumber < 1 || dto.SeatNumber > hall.SeatsPerRow) throw new ArgumentException($"Seat number must be between 1 and {hall.SeatsPerRow}");
            if (dto.RowNumber < 1 || dto.RowNumber > hall.RowsAmount) throw new ArgumentException($"Row number must be between 1 and {hall.RowsAmount}");

            Seat seat = mapper.Map<Seat>(dto);

            await unitOfWork.Seats.CreateAsync(seat, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO?> UpdateAsync(Guid id, SeatUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Seat seat = await unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return null;

            mapper.Map(dto, seat);

            unitOfWork.Seats.Update(seat);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            Seat seat = await unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return false;

            unitOfWork.Seats.Delete(seat);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
