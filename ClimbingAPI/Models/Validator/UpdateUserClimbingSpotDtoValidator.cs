using ClimbingAPI.Models.UserClimbingSpot;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class UpdateUserClimbingSpotDtoValidator: AbstractValidator<UpdateUserClimbingSpotDto>
    {
        public UpdateUserClimbingSpotDtoValidator()
        {
            RuleFor(x => x.ClimbingSpotId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}
