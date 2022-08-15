using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class DeleteUserDtoValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<DeleteUserDto>
            {
                new DeleteUserDto()
                {
                    Password = "test12"
                }
            };

            return list.Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<DeleteUserDto>
            {
                //empty password
                new DeleteUserDto()
                {
                    Password = ""
                },
                 //empty password
                new DeleteUserDto()
                {
                    Password = null
                },
                 //empty password
                new DeleteUserDto()
                {
                }
            };

            return list.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnsSuccess(DeleteUserDto model)
        {
            //arrange
            var validator = new DeleteUserDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(DeleteUserDto model)
        {
            //arrange
            var validator = new DeleteUserDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
