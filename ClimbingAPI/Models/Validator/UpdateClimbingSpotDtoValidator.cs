using ClimbingAPI.Models.ClimbingSpot;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class UpdateClimbingSpotDtoValidator: AbstractValidator<UpdateClimbingSpotDto>
    {
        public UpdateClimbingSpotDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.ContactNumber).MinimumLength(9).MaximumLength(10);
        }
    }
}
