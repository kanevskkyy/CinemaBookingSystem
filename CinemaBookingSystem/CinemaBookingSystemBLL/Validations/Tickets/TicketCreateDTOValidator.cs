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
            RuleFor(p => p.SessionId).GreaterThan(0).WithMessage("SessionId must be greater than 0");

            RuleFor(p => p.SeatId).GreaterThan(0).WithMessage("SeatId must be greater than 0");
        }
    }
}
