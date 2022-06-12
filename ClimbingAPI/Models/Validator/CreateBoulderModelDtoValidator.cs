using ClimbingAPI.Models.Boulder;
using FluentValidation;

namespace ClimbingAPI.Models.Validator
{
    public class CreateBoulderModelDtoValidator: AbstractValidator<CreateBoulderModelDto>
    {
        public CreateBoulderModelDtoValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Level).NotEmpty();
        }
    }
}
