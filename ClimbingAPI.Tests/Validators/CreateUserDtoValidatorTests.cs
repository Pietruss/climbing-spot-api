using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Models.Validator;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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

        [Fact]
        public void Validate_ForValidModel_ReturnsSuccess()
        {
            //arrange
            var model = new CreateUserDto()
            {
                Email = "test@gmail.com",
                FirstName = "Adam",
                LastName = "Qwerty",
                Password = "test12",
                ConfirmPassword = "test12",
                DateOfBirth = new DateTime?(),
                RoleId = 1
            };

            
            var validator = new CreateUserDtoValidator(_dbContext);

            //act
            var result = validator.TestValidate(model);

            //assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Validate_ForInvalidModel_ReturnsFailure()
        {
            //arrange
            var model = new CreateUserDto()
            {
                Email = "test2@gmail.com",
                FirstName = "Adam",
                LastName = "Qwerty",
                Password = "test12",
                ConfirmPassword = "test12",
                DateOfBirth = new DateTime?(),
                RoleId = 1
            };


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
