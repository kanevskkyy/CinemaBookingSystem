using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Halls;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Halls
{
    public class HallUpdateDTOValidator: AbstractValidator<HallUpdateDTO>
    {
        public HallUpdateDTOValidator()
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name cannot be empty!")
                .MaximumLength(60).WithMessage("Hall name length must not exceed 60 characters.");
        }
    }
}
