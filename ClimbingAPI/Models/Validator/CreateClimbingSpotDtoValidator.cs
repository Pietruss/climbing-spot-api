using System.Text.RegularExpressions;
using ClimbingAPI.Models.ClimbingSpot;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class CreateClimbingSpotDtoValidator: AbstractValidator<CreateClimbingSpotDto>
    {
        public CreateClimbingSpotDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
            RuleFor(x => x.ContactEmail).NotEmpty().EmailAddress();
            RuleFor(x => x.City).NotEmpty();
            RuleFor(x => x.Street).NotEmpty();
            RuleFor(x => x.ContactNumber).MinimumLength(9).MaximumLength(10);

        }
    }
}
