using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using FluentValidation;
using System.Linq;

namespace ClimbingAPI.Models.Validator
{
    public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
    {
        public UpdateUserDtoValidator(ClimbingDbContext dbContext)
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.FirstName).NotEmpty();
            RuleFor(x => x.LastName).NotEmpty();
            RuleFor(x => x.DateOfBirth).NotEmpty();
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
