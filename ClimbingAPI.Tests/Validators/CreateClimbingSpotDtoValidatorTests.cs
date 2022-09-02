using ClimbingAPI.Models.ClimbingSpot;
using System.Collections.Generic;
using System.Linq;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class CreateClimbingSpotDtoValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<CreateClimbingSpotDto>
            {
                new CreateClimbingSpotDto()
                {
                    Name = "Forteca",
                    ContactEmail = "test@gmail.com",
                    City = "Cracow",
                    Street = "Wiktorianska",
                    ContactNumber = "+48532557343"
                }
            };

            return list.Select(x => new object[] {x});
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<CreateClimbingSpotDto>
            {
                new CreateClimbingSpotDto()
                {
                    Name = "FortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaFortecaForteca",
                    ContactEmail = "test@gmail.com",
                    City = "Cracow",
                    Street = "Wiktorianska",
                    ContactNumber = "123321123"
                },
                new CreateClimbingSpotDto()
                {
                    Name = "Forteca",
                    ContactEmail = "test@@@gmail.com",
                    City = "Cracow",
                    Street = "Wiktorianska",
                    ContactNumber = "123321123"
                },
                new CreateClimbingSpotDto()
                {
                    Name = "Forteca",
                    ContactEmail = "test@gmail.com",
                    City = "",
                    Street = "Wiktorianska",
                    ContactNumber = "123321123"
                },
                new CreateClimbingSpotDto()
                {
                    Name = "Forteca",
                    ContactEmail = "test@gmail.com",
                    City = "Cracow",
                    Street = "",
                    ContactNumber = "123321123"
                },
                new CreateClimbingSpotDto()
                {
                    Name = "Forteca",
                    ContactEmail = "test@gmail.com",
                    City = "Cracow",
                    Street = "Wikotrianska",
                    ContactNumber = "1"
                },
                new CreateClimbingSpotDto()
            };

            return list.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnsSuccess(CreateClimbingSpotDto model)
        {
            //arrange
            var validator = new CreateClimbingSpotDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(CreateClimbingSpotDto model)
        {
            //arrange
            var validator = new CreateClimbingSpotDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
