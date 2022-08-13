using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class UpdateUserDtoValidatorTests
    {
        private readonly ClimbingDbContext _dbContext;
        public UpdateUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<ClimbingDbContext>();
            builder.UseInMemoryDatabase("UpdateUserDtoValidatorTestsDb");

            _dbContext = new ClimbingDbContext(builder.Options);
            Seed();
        }

        private void Seed()
        {
            var testUsers = new List<User>()
            {
                new User()
                {
                    Email = "test2@gmail.com"
                },
                new User()
                {
                    Email = "test3@gmail.com"
                }
            };

            _dbContext.User.AddRange(testUsers);
            _dbContext.SaveChanges();
        }

        public static IEnumerable<object[]> GetInvalidData()
        {
            var list = new List<UpdateUserDto>
            {
                new UpdateUserDto { },
                new UpdateUserDto
                {//for empty fields
                    DateOfBirth = System.DateTime.Now,
                    Email = "",
                    FirstName = "",
                    LastName = ""
                },
                new UpdateUserDto
                {//for invalid email
                    DateOfBirth = System.DateTime.Now,
                    Email = "w@w@gmail.com",
                    FirstName = "Adam",
                    LastName = "Awss"
                },
                new UpdateUserDto
                {//for invalid DateOfBirth
                    DateOfBirth = null,
                    Email = "test@gmail.com",
                    FirstName = "Qwert",
                    LastName = "Qweret"
                },
                new UpdateUserDto
                {//for taken email
                    DateOfBirth = System.DateTime.Now,
                    Email = "test2@gmail.com",
                    FirstName = "Qwert",
                    LastName = "Qweret"
                },
            };

            return list.Select(x => new object[] { x });
        }

        [Fact]
        public void Validate_ForCorrectModel_ReturnsSuccess()
        {
            //arrange
            var model = new UpdateUserDto()
            {
                DateOfBirth = System.DateTime.Now,
                Email = "nisi2223@gmail.com",
                FirstName = "in",
                LastName = "aliquip nulla222"
            };

            var validator = new UpdateUserDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetInvalidData))]
        public void Validate_ForIncorrectModel_ReturnsFailure(UpdateUserDto model)
        {
            var validator = new UpdateUserDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
