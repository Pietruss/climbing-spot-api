using System.Linq;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class CreateUserDtoValidator: AbstractValidator<CreateUserDto>
    {
        public CreateUserDtoValidator(ClimbingDbContext dbContext)
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(e => e.Password);
            RuleFor(x => x.RoleId).Custom((value, context) =>
            {
                if (!Enumerable.Range(1, 4).Contains(value)) {
                    context.AddFailure("RoleId", "Role does not exists.");
                }
            });

            RuleFor(x => x.Email).Custom((value, context) =>
            {
                var emailInUse = dbContext.User.Any(x => x.Email == value);
                if (emailInUse)
                {
                    context.AddFailure("Email", "That email is taken.");
                }
            });
        }
    }
}
