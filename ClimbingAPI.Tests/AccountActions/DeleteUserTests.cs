using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests.AccountActions
{
    public class DeleteUserTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public DeleteUserTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder => {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service =>
                            service.ServiceType == typeof(DbContextOptions<ClimbingDbContext>));

                        services.Remove(dbContextOptions);
                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                        services.AddMvc(option => option.Filters.Add(new FakeUserAdminRoleFilter()));
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("DeleteUserTests1"));
                    });
                });
            _client = _factory.CreateClient();
        }

        private void Seed(out User user, out DeleteUserDto deleteUserDto, out HttpContent httpContent)
        {
            var createUserDto = new CreateUserDto()
            {
                Password = "test1234",
            };
            user = new User()
            {
                Id = 1,
                Email = "ab@gmail.com",
                PasswordHash = ""
            };

            user.PasswordHash = (new PasswordHasher<User>()).HashPassword(user, createUserDto.Password);
            SeedHelper.SeedUser(user, _factory);

            deleteUserDto = new DeleteUserDto
            {
                Password = "test1234"
            };

            httpContent = deleteUserDto.ToJsonHttpContent();
        }

        [Fact]
        public async Task DeleteUser_ForGivenUser_ReturnsNoContentStatus()
        {
            Seed(out _, out _, out HttpContent httpContent);

            //act
            var result = await _client.PostAsync($"/account/delete-user/1", httpContent);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteUser_ForUserWithExisitingClimbingSpot_ReturnsBadRequest()
        {
            Seed(out User user, out _, out HttpContent httpContent);

            var userClimbingSpot = new ClimbingSpot()
            {
                Id = 1,
                CreatedById = 1
            };

            SeedHelper.SeedClimbingSpot(userClimbingSpot, _factory);
            
            //act
            var result = await _client.PostAsync($"/account/delete-user/1", httpContent);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            SeedHelper.RemoveUsers(user, _factory);
        }    
    }
}
