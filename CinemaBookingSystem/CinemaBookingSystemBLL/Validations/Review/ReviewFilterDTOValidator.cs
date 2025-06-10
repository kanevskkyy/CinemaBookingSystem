using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.Filters;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Review
{
    public class ReviewFilterDTOValidator: AbstractValidator<ReviewFilterDTO>
    {
        public ReviewFilterDTOValidator()
        {
            RuleFor(p => p.MinRating).GreaterThanOrEqualTo(1).WithMessage("The minimum rating for filtering must be equal to or greater than 1 star")
                .LessThanOrEqualTo(5).WithMessage("The maximum rating for filtering must be less than or equal to 5 stars");

            RuleFor(p => p.MaxRating).LessThanOrEqualTo(5).WithMessage("The maximum rating for filtering must be less than or equal to 5 stars")
                .GreaterThanOrEqualTo(1).WithMessage("The maximum rating for filtering must be equal or bigger than 1 star");
        }
    }
}
