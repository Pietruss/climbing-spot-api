using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.UserClimbingSpot;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class UpdateUserClimbingSpotDtoValidator: AbstractValidator<UpdateUserClimbingSpotDto>
    {
        public UpdateUserClimbingSpotDtoValidator(ClimbingDbContext dbContext)
        {
            RuleFor(x => x.ClimbingSpotId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.RoleId).Custom((value, context) =>
            {
                var roleExists = dbContext.Role.Any(x => x.Id == value);
                if(!roleExists)
                    context.AddFailure("Role", "Role does not exists.");
            });
        }
    }
}
