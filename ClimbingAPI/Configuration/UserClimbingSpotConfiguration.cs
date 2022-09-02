using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace ClimbingAPI.Configuration
{
    public class UserClimbingSpotConfiguration : IEntityTypeConfiguration<UserClimbingSpotLinks>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<UserClimbingSpotLinks> builder)
        {
            builder.HasIndex(x => new { x.ClimbingSpotId, x.UserId, x.RoleId});

            builder.HasIndex(x => new { x.ClimbingSpotId, x.UserId});
        }
    }
}
