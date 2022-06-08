using System;
using System.Collections.Generic;
using System.Linq;
using ClimbingAPI.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace ClimbingAPI.Tests
{
    public class StartupTests: IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;
        private readonly List<Type> _controllerTypes;

        public StartupTests(WebApplicationFactory<Startup> factory)
        {
            _controllerTypes = typeof(Startup)
                .Assembly
                .GetTypes()
                .Where(x => x.IsSubclassOf(typeof(ControllerBase)))
                .ToList();

            _factory = factory.WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services => { _controllerTypes.ForEach(c => services.AddScoped(c)); });
                });
        }
        [Fact]
        public void ConfigureServices_ForControllers_RegistersAllDependencies()
        {
            //seed
            var scopedFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();

            _controllerTypes.ForEach(x =>
            {
                var controller = scope.ServiceProvider.GetService(x);
                controller.Should().NotBeNull();
            });
        }
    }
}
