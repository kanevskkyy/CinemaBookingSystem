using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Movies;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Movie
{
    public class MovieCreateDTOValidator : AbstractValidator<MovieCreateDTO>
    {
        public MovieCreateDTOValidator()
        {
            RuleFor(p => p.Title).NotEmpty().WithMessage("Title is required and can not be empty")
                .MaximumLength(90).WithMessage("Title length cannot be bigger than 90 symbols");

            RuleFor(p => p.Description).NotEmpty().WithMessage("Description is required and can not be empty")
                .MaximumLength(3000).WithMessage("Description length should be less than 3000 symbols");

            RuleFor(p => p.Duration).GreaterThan(0).WithMessage("Duration must be greater than 0.");

            RuleFor(p => p.PosterUrl).NotEmpty().WithMessage("Poster URL is required.")
                .MaximumLength(300).WithMessage("Poster URL must not exceed 300 characters.");

            RuleFor(p => p.GenreIds).NotEmpty().WithMessage("At least one GenreId is required.")
                .Must(list => list.All(id => id != Guid.Empty))
                .WithMessage("All GenreIds must be valid GUIDs.");

            RuleFor(p => p.Rating).InclusiveBetween(1, 10).WithMessage("Rating must be between 0.1 and 10.0");
        }
    }

}
