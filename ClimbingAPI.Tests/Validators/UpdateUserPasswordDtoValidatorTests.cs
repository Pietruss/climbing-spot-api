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
                    ConfirmNewPassword = "",
                    NewPassword = "",
                    OldPassword = ""
                },
                new UpdateUserPasswordDto
                {//for not enough characters
                    ConfirmNewPassword = "12345",
                    NewPassword = "12345",
                    OldPassword = "wwwwasd"
                },
                new UpdateUserPasswordDto
                {//for different password
                    ConfirmNewPassword = "12345",
                    NewPassword = "123456",
                    OldPassword = "wwwwasd"
                },
                new UpdateUserPasswordDto
                {//for the same old and new password
                    ConfirmNewPassword = "123456",
                    NewPassword = "123456",
                    OldPassword = "123456"
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
                ConfirmNewPassword = "1234567",
                NewPassword = "1234567"
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
