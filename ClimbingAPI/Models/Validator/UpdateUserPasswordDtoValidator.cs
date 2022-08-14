using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace ClimbingAPI.Models.Validator
{
    public class UpdateUserPasswordDtoValidator : AbstractValidator<UpdateUserPasswordDto>
    {
        public UpdateUserPasswordDtoValidator()
        {
            RuleFor(x => x.NewPassword).MinimumLength(6).NotEmpty();
            RuleFor(x => x.ConfirmNewPassword).Equal(e => e.NewPassword).NotEmpty();
            RuleFor(x => x.OldPassword).NotEqual(x => x.NewPassword);
        }
    }
}
