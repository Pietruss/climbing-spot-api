using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClimbingAPI.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.Property(x => x.Email)
                    .IsRequired();

            builder.Property(x => x.LastName)
                .IsRequired();

            builder.Property(x => x.FirstName)
                .IsRequired();
        }
    }
}
