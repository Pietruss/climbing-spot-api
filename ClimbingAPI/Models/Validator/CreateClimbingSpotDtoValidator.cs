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

            RuleFor(x => x.ContactNumber).Custom((value, context) =>
            {
                Regex validatePhoneNumberRegex = new Regex("^\\+?[1-9][0-9]{7,14}$");
                
                if(string.IsNullOrEmpty(value) || !validatePhoneNumberRegex.IsMatch(value))
                    context.AddFailure("PhoneNumber", "Wrong phone number.");

            });

        }
    }
}
