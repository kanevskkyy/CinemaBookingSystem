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
            RuleFor(p => p.MovieId)
                .Must(id => id != Guid.Empty)
                .WithMessage("MovieId must be a valid GUID.");

            RuleFor(p => p.HallId)
                .Must(id => id != Guid.Empty)
                .WithMessage("HallId must be a valid GUID.");


            RuleFor(p => p.MinPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The minimum ticket price per session must be greater than or equal to 0");

            RuleFor(p => p.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("The maximum ticket price per session must be greater than or equal to 0");

            RuleFor(p => p.SortBy)
                .Must(sortBy => new[] { "starttime", "movieid", "hallid", "price" }.Contains(sortBy?.ToLower()))
                .WithMessage("Sorting can only be by StatTime, MovieID, HallID or Price!");
        }
    }
}