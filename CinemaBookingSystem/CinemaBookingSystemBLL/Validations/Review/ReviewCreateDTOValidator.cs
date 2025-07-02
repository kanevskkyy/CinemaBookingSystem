using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Review;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Review
{
    public class ReviewCreateDTOValidator : AbstractValidator<ReviewCreateDTO>
    {
        public ReviewCreateDTOValidator() 
        {
            RuleFor(p => p.MovieId)
                .NotEmpty()
                .WithMessage("MovieId is required");

            RuleFor(p => p.Text)
                .NotEmpty()
                .WithMessage("The text in the review cannot be empty!")
                .MaximumLength(1000)
                .WithMessage("The maximum length of a review should be less than 1000 characters!");

            RuleFor(p => p.Rating)
                .InclusiveBetween(1, 5)
                .WithMessage("You can give 1 to 5 stars");
        }
    }
}
