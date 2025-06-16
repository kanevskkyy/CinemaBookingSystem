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

        public async Task<List<SeatResponseDTO>> GetByHallIdAsync(Guid hallId, CancellationToken cancellationToken = default)
        {
            List<Seat> seats = await unitOfWork.Seats.GetByHallIdAsync(hallId, cancellationToken);
            return mapper.Map<List<SeatResponseDTO>>(seats);
        }
    }
}
