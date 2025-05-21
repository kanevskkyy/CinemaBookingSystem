using CinemaBookingSystemBLL.DTO.Genres;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Genre
{
    public class GenreCreateDTOValidator: AbstractValidator<GenreCreateDTO>
    {
      public GenreCreateDTOValidator() 
        {
            RuleFor(p => p.Name).NotEmpty().WithMessage("Genre name is required and can not be empty!")
                .MaximumLength(100).WithMessage("Name should be less than 100 symbols!");            
        }
    }
}
