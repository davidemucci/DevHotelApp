using DevHotelAPI.Entities;
using FluentValidation;

namespace DevHotelAPI.Validators
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.ClientId).NotEmpty()
                .WithMessage("Client ID must not be empty.");

            RuleFor(r => r.From).NotEmpty()
                .WithMessage("From date must not be empty.")
                .GreaterThan(DateTime.Now)
                .WithMessage("From date must be in the future.");

            RuleFor(r => r.To).NotEmpty()
                .WithMessage("To date must not be empty.")
                .GreaterThan(DateTime.Now)
                .WithMessage("To date must be in the future.");

            RuleFor(r => r.RoomNumber).NotEmpty()
                .WithMessage("Room number must not be empty.");
        }
    }
}
