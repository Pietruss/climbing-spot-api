using System.Collections.Generic;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.UserClimbingSpot;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization.Policy;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class AssignClimbingSpotEndPointTests: IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public AssignClimbingSpotEndPointTests(WebApplicationFactory<Startup> factory)
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
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ClimbingSpotDb2"));
                    });
                });
            _client = _factory.CreateClient();
        }
        [Fact]
        public async Task AssignClimbingSpotToUser_ForCorrectDataSet_ReturnsOk()
        {
            //arrange

            var userClimbingSpot = CreateCorrectTestData(out var user, out var climbingSpot, out var updateUserClimbingSpotDto);

            //seed
            SeedUserClimbingSpot(userClimbingSpot, user, climbingSpot);

            var httpContent = updateUserClimbingSpotDto.ToJsonHttpContent();

            //act
            var result = await _client.PostAsync("/ClimbingSpot/assign-climbing-spot/", httpContent);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task AssignClimbingSpotToUser_ForNonExistingUser_ReturnsNotFoundException()
        {
            //arrange

            var userClimbingSpot = CreateIncorrectForNonExistingUserTestData(out var user, out var climbingSpot, out var updateUserClimbingSpotDto);

            //seed
            SeedUserClimbingSpot(userClimbingSpot, user, climbingSpot);

            var httpContent = updateUserClimbingSpotDto.ToJsonHttpContent();

            //act
            var result = await _client.PostAsync("/account/assign-climbing-spot/", httpContent);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AssignClimbingSpotToUser_ForNonExistingClimbingSpot_ReturnsNotFoundException()
        {
            //arrange

            var userClimbingSpot = CreateIncorrectForNonExistingClimbingSpotTestData(out var user, out var climbingSpot, out var updateUserClimbingSpotDto);

            //seed
            SeedUserClimbingSpot(userClimbingSpot, user, climbingSpot);

            var httpContent = updateUserClimbingSpotDto.ToJsonHttpContent();

            //act
            var result = await _client.PostAsync("/account/assign-climbing-spot/", httpContent);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task AssignClimbingSpotToUser_ForNonAssignedUserWithAdminOrManagerRoleToClimbingSpot_ReturnsForbiddenException()
        {
            //arrange

            var userClimbingSpot = CreateIncorrectForNotAssignedUserWithAdminOrManagerRoleToClimbingSpotTestData(out var user, out var climbingSpot, out var updateUserClimbingSpotDto);

            //seed
            SeedUserClimbingSpot(userClimbingSpot, user, climbingSpot);

            var httpContent = updateUserClimbingSpotDto.ToJsonHttpContent();

            //act
            var result = await _client.PostAsync("/ClimbingSpot/assign-climbing-spot/", httpContent);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AssignClimbingSpotToUser_ForAlreadyAssignedUser_ReturnsForbiddenException()
        {
            //arrange

            var userClimbingSpot = CreateIncorrectForAlreadyAssignedUserToClimbingSpotTestData(out var user, out var climbingSpot, out var updateUserClimbingSpotDto);

            //seed
            SeedUserClimbingSpot(userClimbingSpot, user, climbingSpot);

            var httpContent = updateUserClimbingSpotDto.ToJsonHttpContent();

            //act
            var result = await _client.PostAsync("/ClimbingSpot/assign-climbing-spot/", httpContent);


            //assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        private static List<UserClimbingSpotLinks> CreateCorrectTestData(out User user, out ClimbingSpot climbingSpot,
            out UpdateUserClimbingSpotDto updateUserClimbingSpotDto)
        {
            var userClimbingSpot = new List<UserClimbingSpotLinks>()
            {
                new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = 2,
                    Id = 2,
                    RoleId = 1,
                    UserId = 1
                }
            };

            user = new User()
            {
                Id = 3,
            };

            climbingSpot = new ClimbingSpot()
            {
                Id = 2,
            };

            updateUserClimbingSpotDto = new UpdateUserClimbingSpotDto
            {
                ClimbingSpotId = 2,
                RoleId = 3,
                UserId = 3
            };
            return userClimbingSpot;
        }

        private static List<UserClimbingSpotLinks> CreateIncorrectForNonExistingUserTestData(out User user, out ClimbingSpot climbingSpot,
            out UpdateUserClimbingSpotDto updateUserClimbingSpotDto)
        {
            var userClimbingSpot = new List<UserClimbingSpotLinks>()
            {
                new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = 5,
                    Id = 30,
                    RoleId = 1,
                    UserId = 1
                }
            };

            user = new User()
            {
                Id = 4,
            };

            climbingSpot = new ClimbingSpot()
            {
                Id = 5,
            };

            updateUserClimbingSpotDto = new UpdateUserClimbingSpotDto
            {
                ClimbingSpotId = 5,
                RoleId = 3,
                UserId = 20
            };
            return userClimbingSpot;
        }

        private static List<UserClimbingSpotLinks> CreateIncorrectForNonExistingClimbingSpotTestData(out User user, out ClimbingSpot climbingSpot,
            out UpdateUserClimbingSpotDto updateUserClimbingSpotDto)
        {
            var userClimbingSpot = new List<UserClimbingSpotLinks>
            {
                new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = 6,
                    Id = 31,
                    RoleId = 1,
                    UserId = 1
                }
            };

            user = new User()
            {
                Id = 1,
            };

            climbingSpot = new ClimbingSpot()
            {
                Id = 6,
            };

            updateUserClimbingSpotDto = new UpdateUserClimbingSpotDto
            {
                ClimbingSpotId = 101,
                RoleId = 3,
                UserId = 1
            };
            return userClimbingSpot;
        }

        private static List<UserClimbingSpotLinks> CreateIncorrectForNotAssignedUserWithAdminOrManagerRoleToClimbingSpotTestData(out User user, out ClimbingSpot climbingSpot,
            out UpdateUserClimbingSpotDto updateUserClimbingSpotDto)
        {
            var userClimbingSpot = new List<UserClimbingSpotLinks>
            {
                new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = 7,
                    Id = 9,
                    RoleId = 1,
                    UserId = 7
                }
            };

            user = new User()
            {
                Id = 7,
            };

            climbingSpot = new ClimbingSpot()
            {
                Id = 7,
            };

            updateUserClimbingSpotDto = new UpdateUserClimbingSpotDto
            {
                ClimbingSpotId = 7,
                RoleId = 3,
                UserId = 7
            };
            return userClimbingSpot;
        }

        private static List<UserClimbingSpotLinks> CreateIncorrectForAlreadyAssignedUserToClimbingSpotTestData(out User user, out ClimbingSpot climbingSpot,
            out UpdateUserClimbingSpotDto updateUserClimbingSpotDto)
        {
            var userClimbingSpot = new List<UserClimbingSpotLinks>
            {
                new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = 8,
                    Id = 33,
                    RoleId = 1,
                    UserId = 8
                },
                new UserClimbingSpotLinks()
                {
                    ClimbingSpotId = 8,
                    Id = 34,
                    RoleId = 1,
                    UserId = 1
                }
            };
            

            user = new User()
            {
                Id = 8,
            };

            climbingSpot = new ClimbingSpot()
            {
                Id = 8,
            };

            updateUserClimbingSpotDto = new UpdateUserClimbingSpotDto
            {
                ClimbingSpotId = 8,
                RoleId = 1,
                UserId = 8
            };
            return userClimbingSpot;
        }

        private void SeedUserClimbingSpot(List<UserClimbingSpotLinks> userClimbingSpot, User user, ClimbingSpot climbingSpot)
        {
            //seed
            var scopedFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.UserClimbingSpotLinks.AddRange(userClimbingSpot);
            dbContext.User.Add(user);
            dbContext.ClimbingSpot.Add(climbingSpot);
            dbContext.SaveChanges();
        }
    }
}
