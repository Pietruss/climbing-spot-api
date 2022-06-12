using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class UpdateUserClimbingSpotDtoValidatorTests
    {
        public static IEnumerable<object[]> GetInvalidData()
        {
            var list = new List<UpdateUserClimbingSpotDto>
            {
                new UpdateUserClimbingSpotDto { },
                new UpdateUserClimbingSpotDto
                {
                    ClimbingSpotId = 1,
                    UserId = 1,
                },
                new UpdateUserClimbingSpotDto
                {
                    ClimbingSpotId = 1,
                    RoleId = 1,
                },
                new UpdateUserClimbingSpotDto
                {
                    UserId = 1,
                    RoleId = 1,
                },
            };

            return list.Select(x => new object[] {x});
        }
        
        [Fact]
        public void Validate_ForCorrectModel_ReturnsSuccess()
        {
            //arrange
            var model = new UpdateUserClimbingSpotDto()
            {
                ClimbingSpotId = 1,
                RoleId = 1,
                UserId = 1
            };

            var validator = new UpdateUserClimbingSpotDtoValidator();
            
            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(UpdateUserClimbingSpotDto model)
        {
            var validator = new UpdateUserClimbingSpotDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
