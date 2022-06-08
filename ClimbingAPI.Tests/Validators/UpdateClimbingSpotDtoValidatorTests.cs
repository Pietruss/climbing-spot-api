using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class UpdateClimbingSpotDtoValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<UpdateClimbingSpotDto>
            {
                new UpdateClimbingSpotDto()
                {
                    Name = "test",
                    ContactEmail = "sw@gmail.com",
                    ContactNumber = "123321123"
                }
            };

            return list.Select(x => new object[] { x });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<UpdateClimbingSpotDto>
            {
                new UpdateClimbingSpotDto()
                {
                    Name = "testtesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttesttest",
                    ContactEmail = "sw@gmail.com",
                    ContactNumber = "123321123"
                },
                new UpdateClimbingSpotDto()
                {
                    Name = "test",
                    ContactEmail = "sw@@gmail.com",
                    ContactNumber = "123321123"
                },
                new UpdateClimbingSpotDto()
                {
                    Name = "test",
                    ContactEmail = "",
                    ContactNumber = "123321123"
                },
                new UpdateClimbingSpotDto()
                {
                    Name = "test",
                    ContactEmail = "sw@gmail.com",
                    ContactNumber = "1233211"
                },
                new UpdateClimbingSpotDto()
                {
                    Name = "test",
                    ContactEmail = "sw@gmail.com",
                    ContactNumber = "123321122222"
                },
                new UpdateClimbingSpotDto()
            };

            return list.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnSuccess(UpdateClimbingSpotDto model)
        {
            //arrange
            var validator = new UpdateClimbingSpotDtoValidator();

            //act
            var result = validator.TestValidate(model);

            //act
            result.ShouldNotHaveAnyValidationErrors();
        }
    }
}
