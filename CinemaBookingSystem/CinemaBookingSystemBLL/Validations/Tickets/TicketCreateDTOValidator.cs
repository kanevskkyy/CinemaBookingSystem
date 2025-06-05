using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Tickets;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Tickets
{
    public class TicketCreateDTOValidator: AbstractValidator<TicketCreateDTO>
    {
        public TicketCreateDTOValidator() 
        {
            RuleFor(p => p.SessionId).NotEmpty().WithMessage("SessionId is required");

            RuleFor(p => p.SeatId).NotEmpty().WithMessage("SeatId is required");
        }
    }
}
