﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Users;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Users
{
    public class UserCreateCustomerDTOValidator : AbstractValidator<UserCreateCustomerDTO>
    {
        public UserCreateCustomerDTOValidator() 
        {
            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(256)
                .WithMessage("Name must not exceed 256 characters.");

            RuleFor(p => p.Email)
                .NotEmpty()
                .WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email must be a valid email address.")
                .MaximumLength(256).WithMessage("Email must not exceed 256 characters.");

            RuleFor(p => p.Password)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(6)
                .WithMessage("Password must be at least 6 characters long.")
                .Matches("[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]")
                .WithMessage("Password must contain at least one number.")
                .Matches("[^a-zA-Z0-9]")
                .WithMessage("Password must contain at least one non-alphanumeric character.");
        }
    }
}
