using ClimbingAPI.Entities;
using ClimbingAPI.Models.ClimbingSpot;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class ClimbingSpotControllerTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        public ClimbingSpotControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory
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
                })
                .CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutParameters_ReturnsOkStatus()
        {
            //act
            var response = await _client.GetAsync("/climbingSpot");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_WithAppropriateIdParameter_ReturnsOkStatus(int id)
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/{id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(10)]
        public async Task Get_WithInvalidIdParameter_RetursnNotFoundStatus(int id)
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/{id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Theory]
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
    }
}
