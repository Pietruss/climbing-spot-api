using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.TestHelper;
using Xunit;

namespace ClimbingAPI.Tests.Validators
{
    public class CreateUserDtoValidatorTests
    {
        private readonly ClimbingDbContext _dbContext;

        public CreateUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<ClimbingDbContext>();
            builder.UseInMemoryDatabase("testDb");

            _dbContext = new ClimbingDbContext(builder.Options);
            Seed();
        }

        public static IEnumerable<object[]> GetSampleOfValidModel()
        {
            var list = new List<CreateUserDto>
            {
                new CreateUserDto()
                {
                    Email = "test@gmail.com",
                    FirstName = "Adam",
                    LastName = "Qwerty",
                    Password = "test12",
                    ConfirmPassword = "test12",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                }
            };

            return list.Select(x => new object[] {x});
        }

        public static IEnumerable<object[]> GetSampleOfInvalidModel()
        {
            var list = new List<CreateUserDto>
            {
                //email is taken
                new CreateUserDto()
                {
                    Email = "test2@gmail.com",
                    FirstName = "Adam",
                    LastName = "Qwerty",
                    Password = "test12",
                    ConfirmPassword = "test12",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                },
                //passwords are not the same
                new CreateUserDto()
                {
                    Email = "test2@gmail.com",
                    FirstName = "Adam",
                    LastName = "Qwerty",
                    Password = "test12",
                    ConfirmPassword = "test123",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                },
                //not enough characters in password
                new CreateUserDto()
                {
                    Email = "test2@gmail.com",
                    FirstName = "Adam",
                    LastName = "Qwerty",
                    Password = "test1",
                    ConfirmPassword = "test1",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                },
                //empty firstName
                new CreateUserDto()
                {
                    Email = "test2@gmail.com",
                    FirstName = "",
                    LastName = "Qwerty",
                    Password = "test123",
                    ConfirmPassword = "test123",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                },
                //empty lastName
                new CreateUserDto()
                {
                    Email = "test2@gmail.com",
                    FirstName = "Qwerty",
                    LastName = "",
                    Password = "test123",
                    ConfirmPassword = "test123",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                },
                //for invalid email
                new CreateUserDto()
                {
                    Email = "tes@t2@gmail.com",
                    FirstName = "Adam",
                    LastName = "Qwerty",
                    Password = "test12",
                    ConfirmPassword = "test123",
                    DateOfBirth = new DateTime?(),
                    RoleId = 1
                }
            };

            return list.Select(x => new object[] { x });
        }

        [Theory]
        [MemberData(nameof(GetSampleOfValidModel))]
        public void Validate_ForValidModel_ReturnsSuccess(CreateUserDto model)
        {
            //arrange
            var validator = new CreateUserDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleOfInvalidModel))]
        public void Validate_ForExistingEmail_ReturnsFailure(CreateUserDto model)
        {
            //arrange
            var validator = new CreateUserDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldHaveAnyValidationError();
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
    }
}
