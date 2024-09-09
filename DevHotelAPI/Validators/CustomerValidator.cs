using DevHotelAPI.Entities;
using FluentValidation;

namespace DevHotelAPI.Validators
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(c => c.Name).MaximumLength(50);
            RuleFor(c => c.Surname).MaximumLength(50);
            RuleFor(c => c.Email)
                .MaximumLength(50)
                .EmailAddress()
                .NotEmpty();
        }

    }
}
