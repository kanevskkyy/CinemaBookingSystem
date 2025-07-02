using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Sessions;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Sessions
{
    public class SessionUpdateDTOValidator: AbstractValidator<SessionUpdateDTO>
    {
        public SessionUpdateDTOValidator()
        {
            RuleFor(p => p.StartTime)
               .Must(start => start > DateTime.Now.ToUniversalTime())
               .WithMessage("StartTime must be in the future.")
               .NotEmpty().WithMessage("StartTime can not be empty");

            RuleFor(p => p.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than 0");
        }
    }
}
