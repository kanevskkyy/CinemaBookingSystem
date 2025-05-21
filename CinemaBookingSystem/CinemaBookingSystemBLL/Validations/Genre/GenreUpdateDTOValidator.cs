using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Genres;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Genre
{
    public class GenreUpdateDTOValidator: AbstractValidator<GenreUpdateDTO>
    {
        public GenreUpdateDTOValidator() {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Genre name is required and can not be empty!")
                .MaximumLength(100).WithMessage("Name should be less than 100 symbols!");
        }
    }
}
