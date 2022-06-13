using ClimbingAPI.Models.ClimbingSpot;
using System.Collections.Generic;
using System.Linq;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class LoginUserDtoValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<LoginUserDto>
            {
                new LoginUserDto()
                {
                    Email = "test@gmail.com",
                    Password = "test12"
                }
            };

            return list.Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<LoginUserDto>
            {
                new LoginUserDto()
                {
                    Email = "test@@gmail.com",
                    Password = "test12"
                },
                new LoginUserDto()
                {
                    Email = "test@gmail.com",
                    Password = ""
                },
                new LoginUserDto()
                {
                    Email = "",
                    Password = "test12"
                },
                new LoginUserDto()
                {
                    Email = "",
                    Password = ""
                },
                new LoginUserDto()
            };

            return list.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnsSuccess(LoginUserDto model)
        {
            //arrange
            var validator = new LoginUserDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(LoginUserDto model)
        {
            //arrange
            var validator = new LoginUserDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
