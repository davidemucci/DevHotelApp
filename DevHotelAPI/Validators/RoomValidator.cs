using DevHotelAPI.Entities;
using FluentValidation;

namespace DevHotelAPI.Validators
{
    public class RoomValidator : AbstractValidator<Room>
    {
        public RoomValidator ()
        {
            RuleFor(r => r.RoomTypeId).GreaterThan(0).NotEmpty().WithMessage("Room Type Number must be greater than 0.");
        }
    }
}
 