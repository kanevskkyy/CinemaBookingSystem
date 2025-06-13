using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.Filters;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Users
{
    public class UserFilterDTOValidator: AbstractValidator<UserFilterDTO>
    {
        public UserFilterDTOValidator()
        {
            RuleFor(p => p.Email).EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(p => p.Role).Must(role => new[] { "customer", "admin" }.Contains(role.ToLower())).WithMessage("Role must be one of the following: Customer, Admin.");
        }
    }
}
