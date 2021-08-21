using System.Runtime.InteropServices.ComTypes;
using FluentValidation;

using VacationRental.Contract.Models;

namespace VacationRental.Contract.Validation
{
    public class BookingBindingModelValidator : AbstractValidator<BookingBindingModel>
    {
        public BookingBindingModelValidator()
        {
            RuleFor(_ => _.Nights).GreaterThan(0);
        }
    }
}