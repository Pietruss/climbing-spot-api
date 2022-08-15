using ClimbingAPI.Models.User;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Models.Validator
{
    public class DeleteUserDtoValidator: AbstractValidator<DeleteUserDto>
    {
        public DeleteUserDtoValidator()
        {
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
