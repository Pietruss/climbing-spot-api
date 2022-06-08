using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Services;
using ClimbingAPI.Services.Interfaces;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class AccountControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        public static readonly object[][] UserTestData =
        {
            new object[] { new DateTime(2017,3,1), "test@gmail.com", "TestName", "TestLastName", 1},
            new object[] { new DateTime(2017, 2, 1), "", "", "", 0 }
        };

        private readonly HttpClient _client;
        private Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();

        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory
                .WithWebHostBuilder(builder => {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service =>
                            service.ServiceType == typeof(DbContextOptions<ClimbingDbContext>));

                        services.Remove(dbContextOptions);
                        services.AddSingleton(_accountServiceMock.Object);
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ClimbingSpotDb"));
                    });
                })
                .CreateClient();
        }

        [Fact]
        public async Task RegisterUser_ForValidModel_ReturnsCreated()
        {
            //arrange
            var user = new CreateUserDto()
            {
                Email = "test32@gmail.com",
                ConfirmPassword = "test12",
                Password = "test12",
                FirstName = "Alan",
                LastName = "Mack",
                RoleId = 1,
                DateOfBirth = DateTime.MaxValue
            };

            var httpContent = user.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/account/register/", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task RegisterUser_ForInvalidModel_ReturnsBadRequest()
        {
            //arrange
            var user = new CreateUserDto()
            {
                Email = "test32@gmail.com",
                ConfirmPassword = "test12",
                Password = "test122",
                FirstName = "Alan",
                LastName = "Mack",
                RoleId = 1,
                DateOfBirth = DateTime.MaxValue
            };

            var httpContent = user.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/account/register/", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Theory]
        [MemberData(nameof(UserTestData))]
        public void GenerateClaims_ForGivenUser_ReturnsFilledValuesInClaimsList(DateTime? dateOfBirth, string email, string firstName, string lastName, int id)
        {
            // arrange
            AccountService accountService = new AccountService();
            User userTemplate = new User
            {
                 DateOfBirth = dateOfBirth,
                 Email = email,
                 FirstName = firstName,
                 LastName = lastName,
                 Id = id
            };
            List<Claim> initialClaimsList = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userTemplate.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{userTemplate.FirstName} {userTemplate.LastName}"),
                new Claim("DateOfBirth", userTemplate.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };
        
            var userId = initialClaimsList[0].Value;
            var userName = initialClaimsList[1].Value;
            var UserDateOfBirth = initialClaimsList[2].Value;
        
        
            // act 
            var claimsList = accountService.GenerateClaims(userTemplate);
        
            // assert
            userId.Should().Be(claimsList.Where(x => x.Type.Contains("nameidentifier")).Select(x => x.Value).First());
            userName.Should().Be(claimsList
                .Where(x => x.Type.Equals("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"))
                .Select(x => x.Value).First());
            UserDateOfBirth.Should()
                .Be(claimsList.Where(x => x.Type.Contains("DateOfBirth")).Select(x => x.Value).First());
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            //arrange
            _accountServiceMock
                .Setup(e => e.GenerateJwt(It.IsAny<LoginUserDto>())).Returns("jwt");

            var loginDto = new LoginUserDto
            {
                Email = "test@gmail.com",
                Password = "test12"
            };

            var httpContent = loginDto.ToJsonHttpContent();

            //act
            var result = await _client.PostAsync("/account/login/", httpContent);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
