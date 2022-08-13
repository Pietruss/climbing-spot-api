using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class UpdateUserPasswordDtoValidatorTests
    {
        public static IEnumerable<object[]> GetInvalidData()
        {
            var list = new List<UpdateUserPasswordDto>
            {
                new UpdateUserPasswordDto { },
                new UpdateUserPasswordDto
                {//for empty fields
                    ConfirmPassword = "",
                    Password = ""
                },
                new UpdateUserPasswordDto
                {//for not enough characters
                    ConfirmPassword = "12345",
                    Password = "12345"
                },
                new UpdateUserPasswordDto
                {//for different password
                    ConfirmPassword = "12345",
                    Password = "123456"
                }
            };

            return list.Select(x => new object[] { x });
        }

        [Fact]
        public void Validate_ForCorrectModel_ReturnsSuccess()
        {
            //arrange
            var model = new UpdateUserPasswordDto()
            {
                ConfirmPassword = "1234567",
                Password = "1234567"
            };

            var validator = new UpdateUserPasswordDtoValidator();

            // act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(UpdateUserPasswordDto model)
        {
            var validator = new UpdateUserPasswordDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
