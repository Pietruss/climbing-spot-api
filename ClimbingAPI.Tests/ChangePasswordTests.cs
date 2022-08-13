using ClimbingAPI.Entities;
using ClimbingAPI.Models.User;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class ChangePasswordTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public ChangePasswordTests(WebApplicationFactory<Startup> factory)
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
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ChangePasswordTests1"));
                    });
                });
            _client = _factory.CreateClient();

            
            
        }

        [Fact]
        public async Task UpdatePassword_ForGivenUser_ReturnsOkStatus()
        {
            var updateUserPasswordDto = new UpdateUserPasswordDto
            {
                ConfirmPassword = "test123",
                Password = "test123"
            };

            var createUserDto = new CreateUserDto()
            {
                Password = "test1234",
                ConfirmPassword = "test1234"
            };

            var user = new User()
            {
                Id = 1,
                Email = "ab@gmail.com",
                PasswordHash = ""
            };

            var httpContent = updateUserPasswordDto.ToJsonHttpContent();

            user.PasswordHash = (new PasswordHasher<User>()).HashPassword(user, createUserDto.Password);
            SeedHelper.SeedUser(user, _factory);

            //act
            var result = await _client.PatchAsync($"/account/change-password/1", httpContent);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            SeedHelper.RemoveUsers(user, _factory);
        }

        [Fact]
        public async Task UpdatePassword_ForTheSameOldAndNewPassword_ReturnsBadRequest()
        {
            var updateUserPasswordDto = new UpdateUserPasswordDto
            {
                ConfirmPassword = "test123",
                Password = "test123"
            };

            var createUserDto = new CreateUserDto()
            {
                Password = "test123",
                ConfirmPassword = "test123"
            };

            var user = new User()
            {
                Id = 1,
                Email = "ab@gmail.com"
            };

            user.PasswordHash = (new PasswordHasher<User>()).HashPassword(user, createUserDto.Password);
            SeedHelper.SeedUser(user, _factory);

            var httpContent = updateUserPasswordDto.ToJsonHttpContent();

            //act
            var result = await _client.PatchAsync($"/account/change-password/{user.Id}", httpContent);

            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
            SeedHelper.RemoveUsers(user, _factory);
        }
    }
}
