using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Seats;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Seats
{
    public class SeatCreateDTOValidator:AbstractValidator<SeatCreateDTO>
    {
        public SeatCreateDTOValidator()
        {
            RuleFor(p => p.HallId).GreaterThan(0).WithMessage("HallId must be greater than 0");

            RuleFor(p => p.RowNumber).GreaterThan(0).WithMessage("Row number must be greater than 0");

            RuleFor(p => p.SeatNumber).GreaterThan(0).WithMessage("Seat number must be greater than 0");
        }
    }
}
