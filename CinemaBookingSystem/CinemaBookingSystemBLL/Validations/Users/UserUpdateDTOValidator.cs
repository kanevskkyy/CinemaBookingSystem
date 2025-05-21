using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Users
{
    public class UserUpdateDTOValidator: AbstractValidator<UserUpdateDTO>
    {
        public UserUpdateDTOValidator() 
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required.")
               .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(p => p.Email).NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.");

            RuleFor(p => p.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Must(role => new[] { "Customer", "Admin" }.Contains(role))
                .WithMessage("Role must be one of the following: Customer, Admin, Manager.");
        }
    }
}
