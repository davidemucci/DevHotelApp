using DevHotelAPI.Dtos;
using DevHotelAPI.Entities;
using FluentValidation;
using FluentValidation.Results;

namespace DevHotelAPI.Validators
{
    public class RoomTypeDtoValidator : AbstractValidator<RoomTypeDto>
    {
        public RoomTypeDtoValidator()
        {
            RuleFor(x => x.Id).Must(id => id > 0);
            RuleFor(x => x.TotalNumber)
                .GreaterThanOrEqualTo(0).WithMessage("TotalNumber must be zero or greater.");
            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .Length(3, 50).WithMessage("Description must be between 3 and 50 characters.");
        }
    }
}
