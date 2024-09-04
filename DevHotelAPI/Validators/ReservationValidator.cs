using DevHotelAPI.Entities;
using FluentValidation;

namespace DevHotelAPI.Validators
{
    public class ReservationValidator : AbstractValidator<Reservation>
    {
        public ReservationValidator()
        {
            RuleFor(r => r.ClientId).NotEmpty();    
            RuleFor(r => r.From).NotEmpty()
                .GreaterThan(DateTime.Now);
            RuleFor(r => r.To).NotEmpty()
                .GreaterThan(DateTime.Now);
            RuleFor(r => r.ClientId).NotEmpty();
            RuleFor(r => r.RoomNumber).NotEmpty();
            
        }
    }
}
