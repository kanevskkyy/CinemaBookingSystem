using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Authorization;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Auth
{
    public class TokenRefreshDTOValidator : AbstractValidator<TokenRefreshDTO>
    {
        public TokenRefreshDTOValidator() 
        {
            RuleFor(p => p.AccessToken)
                .NotEmpty()
                .WithMessage("Access Token is required and cannot be empty");

            RuleFor(p => p.RefreshToken)
                .NotEmpty()
                .WithMessage("Refresh Token is required and cannot be empty");
        }
    }
}
