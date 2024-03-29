﻿using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Boulder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClimbingAPI.Tests.Helpers
{
    public static class SeedHelper
    {
        public static void SeedClimbingSpot(ClimbingSpot climbingSpot, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.ClimbingSpot.Add(climbingSpot);
            dbContext.SaveChanges();
        }

        public static void SeedUserClimbingSpot(UserClimbingSpotLinks userClimbingSpot, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.UserClimbingSpotLinks.Add(userClimbingSpot);
            dbContext.SaveChanges();
        }

        public static void SeedUserClimbingSpotAndUserAndClimbingSpot(List<UserClimbingSpotLinks> userClimbingSpot, User user, ClimbingSpot climbingSpot, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.UserClimbingSpotLinks.AddRange(userClimbingSpot);
            dbContext.User.Add(user);
            dbContext.ClimbingSpot.Add(climbingSpot);
            dbContext.SaveChanges();
        }

        public static void SeedUser(User user, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.User.Add(user);
            dbContext.SaveChanges();
        }

        internal static void RemoveUsers(User user, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.User.Remove(user);
            dbContext.SaveChanges();
        }

        public static void SeedImage(Image image, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.Images.Add(image);
            dbContext.SaveChanges();
        }

        public static void SeedBoulder(Boulder boulder, WebApplicationFactory<Startup> factory)
        {
            var scopedFactory = factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopedFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetService<ClimbingDbContext>();

            dbContext.Boulder.Add(boulder);
            dbContext.SaveChanges();
        }
    }
}
