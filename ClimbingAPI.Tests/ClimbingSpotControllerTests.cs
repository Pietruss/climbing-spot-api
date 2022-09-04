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
                Id = 55,
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
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

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
        public async Task Create_WithoutPermission_ReturnsUnAuthorizeStatus()
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
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_WithValidModel_ReturnsBadRequest()
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

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 1,
                Id = 1,
                RoleId = 1,
                UserId = 1
            };

            //seed
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync("/climbingspot", httpContent);

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
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

        //problem with InMemoryDbContext - linq2db - does not support this kind of db
        //[Fact]
        //public async Task Delete_ForClimbingSpotOwner_ReturnsNoContent()
        //{
        //    //arrange
        //    var climbingSpot = new ClimbingSpot()
        //    {
        //        Id = 5,
        //        CreatedById = 1
        //    };

        //    var userClimbingSpotLink = new UserClimbingSpotLinks()
        //    {
        //        Id = 5,
        //        UserId = 1,
        //        ClimbingSpotId = 5,
        //        ModificationDateTime = System.DateTime.Now,
        //        RoleId = 2,
        //        CreatedByUserId = "1",
        //        ModifiedByUserId = "1"
        //    };

        //    //seed
        //    SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
        //    SeedHelper.SeedUserClimbingSpot(userClimbingSpotLink, _factory);

        //    //act
        //    var response = await _client.DeleteAsync($"/climbingspot/{climbingSpot.Id}");

        //    //asset
        //    response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        //}

        [Fact]
        public async Task Delete_ForNonClimbingSpotOwner_ReturnsUnAuthorizeStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                CreatedById = 2
            };

            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/climbingspot/{climbingSpot.Id}");

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Theory]
        [InlineData(10)]
        public async Task Delete_WithInvalidClimbingSpotId_ReturnsUnAuthorizeStatus(int id)
        {
            //act
            var response = await _client.DeleteAsync($"/climbingspot/{id}");

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ForGivenDataSet_ReturnsOkStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                CreatedById = 1
            };

            var updateClimbingSpot = new UpdateClimbingSpotDto()
            {
                ContactEmail = "updated@gmail.com",
                ContactNumber = "111222333",
                Description = "updated",
                Name = "NameUpdated!"
            };

            var httpContent = HttpContentHelper.ToJsonHttpContent(updateClimbingSpot);

            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.PatchAsync($"/climbingspot/{climbingSpot.Id}", httpContent);

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_ForNonClimbingSpotOwner_ReturnsUnAuthorizeStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                CreatedById = 2
            };

            var updateClimbingSpot = new UpdateClimbingSpotDto()
            {
                ContactEmail = "updated@gmail.com",
                ContactNumber = "111222333",
                Description = "updated",
                Name = "NameUpdated!"
            };

            var httpContent = HttpContentHelper.ToJsonHttpContent(updateClimbingSpot);

            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.PatchAsync($"/climbingspot/{climbingSpot.Id}", httpContent);

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ForNotValidClimbingSpotId_ReturnsUnAuthorizeStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 205,
                CreatedById = 2
            };

            var updateClimbingSpot = new UpdateClimbingSpotDto()
            {
                ContactEmail = "updated@gmail.com",
                ContactNumber = "111222333",
                Description = "updated",
                Name = "NameUpdated!"
            };

            var httpContent = HttpContentHelper.ToJsonHttpContent(updateClimbingSpot);

            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.PatchAsync($"/climbingspot/{climbingSpot.Id + 1}", httpContent);

            //asset
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}

