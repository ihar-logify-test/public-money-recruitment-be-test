using FluentValidation;

using VacationRental.Contract.Models;

namespace VacationRental.Contract.Validation
{
    public class RentalBindingModelValidator : AbstractValidator<RentalBindingModel>
    {
        public RentalBindingModelValidator()
        {
            RuleFor(_ => _.Units).GreaterThan(0);
        }
    }
}