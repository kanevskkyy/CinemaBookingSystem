using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Halls;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Halls
{
    public class HallCreateDTOValidator: AbstractValidator<HallCreateDTO>
    {
        public HallCreateDTOValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty!")
                .MaximumLength(60)
                .WithMessage("Hall name length must not exceed 60 characters.");

            RuleFor(p => p.RowsAmount)
                .GreaterThan(0)
                .WithMessage("Row amount must be greater than 0");

            RuleFor(p => p.SeatsPerRow)
                .GreaterThan(0)
                .WithMessage("Seats per row must be greater than 0");
        }
    }
}
