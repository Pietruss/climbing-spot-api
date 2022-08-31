using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Boulder;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


namespace ClimbingAPI.Tests
{
    public class ImageControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public ImageControllerTests(WebApplicationFactory<Startup> factory)
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
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ClimbingSpotDbImageController"));
                    });
                });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_ForExisitingsImage_ReturnsOkStatus()
        {
            var image = new Image()
            {
                Id = 1,
                ImageData = new byte[2]
            };
            SeedHelper.SeedImage(image, _factory);   
            //act
            var response = await _client.GetAsync($"/image/1");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task Get_ForNonExisitingsImage_ReturnsNotFoundStatus()
        {
            //act
            var response = await _client.GetAsync($"/image/22");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact]
        public async Task Delete_ForExisitingsImage_ReturnsNoContentStatus()
        {
            var image = new Image()
            {
                Id = 2,
                ImageData = new byte[2],
                BoulderId = 1
            };

            var boulder = new Boulder()
            {
                Id = 1,
                ClimbingSpotId = 1
            };

            var climbingSpot = new ClimbingSpot()
            {
                Id = 1
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                Id = 1,
                ClimbingSpotId = 1,
                UserId = 1,
                RoleId = 2
            };

            SeedHelper.SeedImage(image, _factory);
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedBoulder(boulder, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/boulder/{boulder.Id}/image/{image.Id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        }

        [Fact]
        public async Task Delete_ForNonExisitingsImage_ReturnsNotFoundStatus()
        {
            var boulder = new Boulder()
            {
                Id = 3,
                ClimbingSpotId = 3
            };

            var climbingSpot = new ClimbingSpot()
            {
                Id = 3
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                Id = 2,
                ClimbingSpotId = 3,
                UserId = 1,
                RoleId = 3
            };

            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedBoulder(boulder, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/boulder/{boulder.Id}/image/5");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact]
        public async Task Delete_ForExisitingsImageWithNotEnoughRights_ReturnsUnAuthorizedStatus()
        {
            var boulder = new Boulder()
            {
                Id = 4,
                ClimbingSpotId = 4
            };

            var climbingSpot = new ClimbingSpot()
            {
                Id = 4
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                Id = 3,
                ClimbingSpotId = 4,
                UserId = 1,
                RoleId = 4
            };

            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedBoulder(boulder, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/boulder/{boulder.Id}/image/5");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);

        }
    }
}
