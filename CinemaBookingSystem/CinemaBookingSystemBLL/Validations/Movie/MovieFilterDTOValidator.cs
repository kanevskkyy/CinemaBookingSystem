using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using CinemaBookingSystemBLL.Filters;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Movie
{
    public class MovieFilterDTOValidator : AbstractValidator<MovieFilterDTO>
    {
        public MovieFilterDTOValidator()
        {
            RuleFor(p => p.MinRating).GreaterThanOrEqualTo(2).WithMessage("The minimum movie rating for filtering must be greater than or equal to 0");

            RuleFor(p => p.MaxRating).GreaterThanOrEqualTo(2).WithMessage("The maximum movie rating for filtering must be greater than or equal to 1");

            RuleFor(p => p.MinDuration).GreaterThanOrEqualTo(0).WithMessage("The minimum duration for movie for filtering must be greater than or equal to 0");
            
            RuleFor(p => p.MaxDuration).GreaterThanOrEqualTo(1).WithMessage("The maximum duration for movie for filtering must be greater than or equal to 1");
        }
    }
}
