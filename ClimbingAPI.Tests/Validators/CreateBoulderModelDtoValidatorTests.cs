using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class CreateBoulderModelDtoValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<CreateBoulderModelDto>()
            {
                new CreateBoulderModelDto()
                {
                    CreatedById = 1,
                    Description = "hardcore",
                    Level = "4",
                    Name = "w-8"
                },
                new CreateBoulderModelDto()
                {
                    Level = "4",
                    Name = "w-8"
                },
            };

            return list.Select(x => new object[] {x});
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<CreateBoulderModelDto>()
            {
                new CreateBoulderModelDto()
                {
                    CreatedById = 1,
                    Description = "hardcore",
                },
                new CreateBoulderModelDto()
                {
                    Level = "",
                    Name = "w-8"
                },
                new CreateBoulderModelDto()
                {
                    Level = "",
                    Name = ""
                },
                new CreateBoulderModelDto()
                {
                    Level = null,
                    Name = null
                },
                new CreateBoulderModelDto()
            };

            return list.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnsSuccess(CreateBoulderModelDto model)
        {
            //arrange
            var validator = new CreateBoulderModelDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(CreateBoulderModelDto model)
        {
            //arrange
            var validator = new CreateBoulderModelDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
