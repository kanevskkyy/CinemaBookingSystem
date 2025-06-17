using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.Filters;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Tickets
{
    public class TicketFilterDTOValidator : AbstractValidator<TicketFilterDTO>
    {
        public TicketFilterDTOValidator() 
        {
            RuleFor(p => p.SessionId)
                .Must(id => id != Guid.Empty).WithMessage("SessionId must be a valid GUID.");

            RuleFor(p => p.SeatId)
                .Must(id => id != Guid.Empty).WithMessage("SeatId must be a valid GUID.");
        }
    }
}
