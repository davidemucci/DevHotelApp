using DevHotelAPI.Entities;
using FluentValidation;

namespace DevHotelAPI.Validators
{
    public class ClientValidator : AbstractValidator<Client>
    {
        public ClientValidator()
        {
            RuleFor(c => c.Name).MaximumLength(50);
            RuleFor(c => c.Surname).MaximumLength(50);
            RuleFor(c => c.Email)
                .MaximumLength(50)
                .EmailAddress()
                .NotEmpty();
            RuleFor(c => c.Password).MinimumLength(8);
        }

    }
}
