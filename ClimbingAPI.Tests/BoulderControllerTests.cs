using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Address;
using ClimbingAPI.Entities.Boulder;
using ClimbingAPI.Models.Boulder;
using ClimbingAPI.Tests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace ClimbingAPI.Tests
{
    public class BoulderControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;
        public BoulderControllerTests(WebApplicationFactory<Startup> factory)
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
                        services.AddDbContext<ClimbingDbContext>(options => options.UseInMemoryDatabase("ClimbingSpotDbBoulderController"));
                    });
                });
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_WithExisitingBoulderAndClimbingSpot_ReturnsOkStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 501,
                Address = new Address()
                {
                    Id = 501
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 501
                    }
                }
            };
            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.GetAsync($"/climbingSpot/{climbingSpot.Id}/boulder/{climbingSpot.Boulder[0].Id}");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Get_WithNotExisitingClimbingSpot_ReturnsNotFoundStatus()
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/502/boulder/1");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Get_WithNotExisitingBoulder_ReturnsNotFoundStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 502,
                Address = new Address()
                {
                    Id = 502
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 502
                    }
                }
            };
            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.GetAsync($"/climbingSpot/{climbingSpot.Id}/boulder/1");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAll_ForGivenClimbingSpot_ReturnsOkStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 503,
                Address = new Address()
                {
                    Id = 503
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 503
                    },
                    new Boulder()
                    {
                        Id = 504
                    }
                }
            };
            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.GetAsync($"/climbingSpot/{climbingSpot.Id}/boulder");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        }

        [Fact]
        public async Task GetAll_ForNotExisitingsClimbingSpot_ReturnsNotFoundStatus()
        {
            //act
            var response = await _client.GetAsync($"/climbingSpot/22/boulder");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        }

        [Fact]
        public async Task Delete_ForGivenClimbingSpotAndBoulder_ReturnsNoContentStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 504,
                Address = new Address()
                {
                    Id = 504
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 505
                    },
                    new Boulder()
                    {
                        Id = 506
                    }
                }
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 504,
                Id = 1,
                RoleId = 1,
                UserId = 1
            };

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/climbingSpot/{climbingSpot.Id}/boulder/505");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNotExistingClimbingSpot_ReturnsUnauthorizedStatus()
        {
            //act
            var response = await _client.DeleteAsync($"/climbingSpot/505/boulder/505");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Delete_ForNotExistingBoulder_ReturnsNotFoundStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 518,
                Address = new Address()
                {
                    Id = 518
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 518
                    },
                    new Boulder()
                    {
                        Id = 519
                    }
                }
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 518,
                Id = 2,
                RoleId = 1,
                UserId = 1
            };

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/climbingSpot/{climbingSpot.Id}/boulder/510");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAll_ForGivenClimbingSpot_ReturnsNoContentStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 519,
                Address = new Address()
                {
                    Id = 519
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 520
                    },
                    new Boulder()
                    {
                        Id = 521
                    }
                }
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 519,
                Id = 3,
                RoleId = 1,
                UserId = 1
            };

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.DeleteAsync($"/climbingSpot/{climbingSpot.Id}/boulder");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteAll_ForNotExistingClimbingSpot_ReturnsUnAuthorizedStatus()
        {
            //act
            var response = await _client.DeleteAsync($"/climbingSpot/555/boulder");

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ForGivenClimbingSpotAndBoulder_ReturnsOkStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 522,
                Address = new Address()
                {
                    Id = 522
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 523,
                        Name = "Wszystko",
                        Level = "5d",
                        Description = "qwerty"
                    },
                    new Boulder()
                    {
                        Id = 524
                    }
                }
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 522,
                Id = 4,
                RoleId = 1,
                UserId = 1
            };

            var boulderToUpdate = new UpdateBoulderDto()
            {
                Description = "test",
                Level = "22",
                Name = "test22"
            };

            var httpContent = boulderToUpdate.ToJsonHttpContent();

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.PatchAsync($"/climbingSpot/{climbingSpot.Id}/boulder/{climbingSpot.Boulder[0].Id}", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Update_ForNotExisitingBoulder_ReturnsUnauthorizedStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 523,
                Address = new Address()
                {
                    Id = 523
                },
                Boulder = new List<Boulder>()
                { }
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 523,
                Id = 5,
                RoleId = 1,
                UserId = 1
            };

            var boulderToUpdate = new UpdateBoulderDto()
            {
                Description = "test",
                Level = "22",
                Name = "test22"
            };

            var httpContent = boulderToUpdate.ToJsonHttpContent();

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.PatchAsync($"/climbingSpot/{climbingSpot.Id}/boulder/432", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Update_ForNotExisitingClimbingSpot_ReturnsUnauthorizedStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 533,
                Address = new Address()
                {
                    Id = 533
                },
                Boulder = new List<Boulder>()
                {
                    new Boulder()
                    {
                        Id = 432
                    }
                }
            };

            var boulderToUpdate = new UpdateBoulderDto()
            {
                Description = "test",
                Level = "22",
                Name = "test22"
            };

            var httpContent = boulderToUpdate.ToJsonHttpContent();

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);

            //act
            var response = await _client.PatchAsync($"/climbingSpot/{climbingSpot.Id}/boulder/432", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Create_ForGivenDataset_ReturnsOkStatus()
        {
            //arrange
            var climbingSpot = new ClimbingSpot()
            {
                Id = 524,
                Address = new Address()
                {
                    Id = 524
                },
                Boulder = new List<Boulder>()
                { }
            };

            var userClimbingSpot = new UserClimbingSpotLinks()
            {
                ClimbingSpotId = 524,
                Id = 6,
                RoleId = 1,
                UserId = 1
            };

            var boulderToCreate = new CreateBoulderModelDto()
            {
                Name = "wsad",
                Level = "1",
                Description = "ww",
                CreatedById = 1
            };

            var httpContent = boulderToCreate.ToJsonHttpContent();

            //seed
            SeedHelper.SeedClimbingSpot(climbingSpot, _factory);
            SeedHelper.SeedUserClimbingSpot(userClimbingSpot, _factory);

            //act
            var response = await _client.PostAsync($"/climbingSpot/{climbingSpot.Id}/boulder", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Create_ForNotExistingClimbingSpot_ReturnsUnauthorizeStatus()
        {
            var boulderToCreate = new CreateBoulderModelDto()
            {
                Name = "wsad",
                Level = "1",
                Description = "ww",
                CreatedById = 1
            };


            var httpContent = boulderToCreate.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync($"/climbingSpot/232/boulder", httpContent);

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

    }
}


