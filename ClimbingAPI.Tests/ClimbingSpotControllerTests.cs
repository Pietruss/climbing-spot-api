using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Threading.Tasks;
using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ClimbingSpotDb"));
                    });
                })
                .CreateClient();
        }

        [Fact]
        public async Task GetAll_WithoutParameters_ReturnOkResult()
        {
            //act
            var response = await _client.GetAsync("/climbingSpot");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(1)]
        public async Task Get_WithAppropriateIdParameter_ReturnOkResult(int id)
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/{id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(10000)]
        public async Task Get_WithInvalidIdParameter_ReturnNotFoundResult(int id)
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/{id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }
    }
}
