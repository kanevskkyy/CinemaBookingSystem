using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBookingSystemBLL.DTO.Payment;
using FluentValidation;

namespace CinemaBookingSystemBLL.Validations.Payment
{
    public class PaymentConfirmDTOValidator : AbstractValidator<PaymentConfirmDTO>
    {
        public PaymentConfirmDTOValidator() 
        {
            RuleFor(p => p.PaymentMethod)
                .NotEmpty()
                .WithMessage("Payment method cannot be empty")
                .Must(paymentMethod => paymentMethod == "Manual" || paymentMethod == "Card")
                .WithMessage("Payment method must be either 'Manual' or 'Card'");
        }
    }
}
