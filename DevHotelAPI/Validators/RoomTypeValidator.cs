using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace DevHotelAPI.Validators
{
    public class RoomTypeValidator : AbstractValidator<RoomType>
    {
        public RoomTypeValidator()
        {
/*            RuleFor(x => x.Id).Must(id => id > 0);*/
            RuleFor(x => x.TotalNumber)
                .NotNull()
                .Must(x => x > 0).WithMessage("TotalNumber must be greater than one.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(3, 50).WithMessage("Description must be between 3 and 50 characters.");
        }
    }
}
