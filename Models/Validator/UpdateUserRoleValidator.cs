using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.User;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class UpdateUserRoleValidator: AbstractValidator<UpdateUserRoleDto>
    {
        public UpdateUserRoleValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty();
        }
    }
}
