using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Authorization;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Refresh
{
    public class TokenRefreshRequestValidator : AbstractValidator<TokenRefreshRequest>
    {
        public TokenRefreshRequestValidator() 
        {
            RuleFor(p => p.AccessToken).NotEmpty().WithMessage("Access Token is required");

            RuleFor(p => p.RefreshToken).NotEmpty().WithMessage("Refresh Token is required");
        }
    }
}
