using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Sessions;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Sessions
{
    public class SessionCreateDTOValidator: AbstractValidator<SessionCreateDTO>
    {
        public SessionCreateDTOValidator() {
            RuleFor(p => p.MovieId)
                .NotEmpty()
                .WithMessage("MovieId is required");

            RuleFor(p => p.HallId)
                .NotEmpty()
                .WithMessage("HallId is required");

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
