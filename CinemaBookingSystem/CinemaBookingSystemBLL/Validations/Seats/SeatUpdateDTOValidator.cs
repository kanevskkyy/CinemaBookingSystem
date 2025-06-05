using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Seats;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Seats
{
    public class SeatUpdateDTOValidator: AbstractValidator<SeatUpdateDTO>
    {
        public SeatUpdateDTOValidator()
        {
            RuleFor(p => p.HallId).NotEmpty().WithMessage("HallId is required");

            RuleFor(p => p.RowNumber).GreaterThan(0).WithMessage("Row number must be greater than 0");

            RuleFor(p => p.SeatNumber).GreaterThan(0).WithMessage("Seat number must be greater than 0");
        }
    }
}
