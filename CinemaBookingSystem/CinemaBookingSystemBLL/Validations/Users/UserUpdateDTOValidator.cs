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
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(100)
                .WithMessage("Name must not exceed 100 characters.");
        }
    }
}
