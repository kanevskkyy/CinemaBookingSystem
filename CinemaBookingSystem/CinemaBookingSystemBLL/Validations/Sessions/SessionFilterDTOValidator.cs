using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.Filters;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Sessions
{
    public class SessionFilterDTOValidator: AbstractValidator<SessionFilterDTO>
    {
        public SessionFilterDTOValidator() 
        {
            RuleFor(p => p.MinPrice).GreaterThanOrEqualTo(0).WithMessage("The minimum ticket price per session must be greater than or equal to 0");

            RuleFor(p => p.MaxPrice).GreaterThanOrEqualTo(0).WithMessage("The maximum ticket price per session must be greater than or equal to 0");
        }
    }
}