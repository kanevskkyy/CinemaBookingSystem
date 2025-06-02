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

        public async Task<List<SeatResponseDTO>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var seats = await unitOfWork.Seats.GetAllAsync(cancellationToken);
            var orderedSeats = seats.OrderBy(m => m.Id);

            return mapper.Map<List<SeatResponseDTO>>(orderedSeats);
        }

        public async Task<PagedList<SeatResponseDTO>> GetPagedSeatsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = unitOfWork.Seats.GetAll();
            PagedList<Seat> pagedSeats = await PagedList<Seat>.ToPagedListAsync(query, pageNumber, pageSize, cancellationToken);
            List<SeatResponseDTO> seatDtos = mapper.Map<List<SeatResponseDTO>>(pagedSeats.Items);

            return new PagedList<SeatResponseDTO>(seatDtos, pagedSeats.TotalCount, pagedSeats.CurrentPage, pagedSeats.PageSize);
        }

        public async Task<SeatResponseDTO?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            Seat seat = await unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return null;

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<List<SeatResponseDTO>> GetByHallIdAsync(int hallId, CancellationToken cancellationToken = default)
        {
            var seats = await unitOfWork.Seats.GetByHallIdAsync(hallId, cancellationToken);
            return mapper.Map<List<SeatResponseDTO>>(seats);
        }

        public async Task<SeatResponseDTO?> GetByRowAndNumberAsync(int hallId, int rowNumber, int seatNumber, CancellationToken cancellationToken = default)
        {
            Seat? seat = await unitOfWork.Seats.GetByRowAndNumberAsync(hallId, rowNumber, seatNumber, cancellationToken);
            if (seat == null) return null;

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO> CreateAsync(SeatCreateDTO dto, CancellationToken cancellationToken = default)
        {
            Seat seat = mapper.Map<Seat>(dto);

            await unitOfWork.Seats.CreateAsync(seat, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<SeatResponseDTO?> UpdateAsync(int id, SeatUpdateDTO dto, CancellationToken cancellationToken = default)
        {
            Seat seat = await unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return null;

            mapper.Map(dto, seat);

            unitOfWork.Seats.Update(seat);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return mapper.Map<SeatResponseDTO>(seat);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            Seat seat = await unitOfWork.Seats.GetByIdAsync(id, cancellationToken);
            if (seat == null) return false;

            unitOfWork.Seats.Delete(seat);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
