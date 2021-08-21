using FluentValidation;

using VacationRental.Contract.Models;

namespace VacationRental.Contract.Validation
{
    public class GetCalendarModelValidator : AbstractValidator<GetCalendarModel>
    {
        public GetCalendarModelValidator()
        {
            RuleFor(_ => _.Nights).GreaterThan(0);
        }
    }
}