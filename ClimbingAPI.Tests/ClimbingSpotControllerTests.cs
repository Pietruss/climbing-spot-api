using System.Collections.Generic;
using ClimbingAPI.Entities;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ClimbingAPI.Entities.Address;
using ClimbingAPI.Entities.Boulder;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class ClimbingSpotControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;
        public ClimbingSpotControllerTests(WebApplicationFactory<Startup> factory)
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
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ClimbingSpotDb"));
                    });
                });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutParameters_ReturnsOkStatus()
        {
            //act
            var response = await _client.GetAsync("/climbingSpot");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithAppropriateIdParameter_ReturnsOkStatus()
        {
            var climbingSpot = new ClimbingSpot()
            {
                Id = 2000,
                Address = new Address()
                {
                    Id = 3
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 3
                    }
                }
            };
            //seed
            SeedClimbingSpot(climbingSpot);

            //act
            var response = await _client.GetAsync($"/climbingSpot/{climbingSpot.Id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(10000)]
        public async Task Get_WithInvalidIdParameter_ReturnsNotFoundStatus(int id)
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/{id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Create_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange
            var model = new CreateClimbingSpotDto()
            {
                Name = "Forteca",
                Description = "test123",
                City = "Krakow",
                ContactEmail = "test@gmail.com",
                ContactNumber = "123321123",
                PostalCode = "29-292",
                Street = "Kompozytorow 19"
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/climbingspot", httpContent);

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }


        [Fact]
        public async Task Create_WithInvalidModel_ReturnsBadRequestStatus()
        {
            //arrange
            var model = new CreateClimbingSpotDto()
            {
                Name = "",
                City = "",
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/climbingspot", httpContent);

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ForClimbingSpotOwner_ReturnsNoContent()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                CreatedById = 1
            };

            //seed
            SeedClimbingSpot(climbingSpot);

            //act
            var response = await _client.DeleteAsync($"/climbingspot/{climbingSpot.Id}");

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                CreatedById = 2
            };

            SeedClimbingSpot(climbingSpot);

            //act
            var response = await _client.DeleteAsync($"/climbingspot/{climbingSpot.Id}");

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Theory]
        [InlineData(10)]
        public async Task Delete_WithInvalidClimbingSpotId_ReturnsNotFound(int id)
        {
            //act
            var response = await _client.DeleteAsync($"/climbingspot/{id}");

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        private void SeedClimbingSpot(ClimbingSpot climbingSpot)
        {
            //seed
            var scopedFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.ClimbingSpot.Add(climbingSpot);
            dbContext.SaveChanges();
        }
    }
}

