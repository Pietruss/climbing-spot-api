using ClimbingAPI.Models.User;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class LoginUserDtoValidator: AbstractValidator<LoginUserDto>
    {
        public LoginUserDtoValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}
