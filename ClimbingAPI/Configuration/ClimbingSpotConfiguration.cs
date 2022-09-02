using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClimbingAPI.Configuration
{
    public class ClimbingSpotConfiguration : IEntityTypeConfiguration<ClimbingSpot>
    {
        public void Configure(EntityTypeBuilder<ClimbingSpot> builder)
        {
            builder.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

            builder.Property(x => x.ContactEmail)
                .IsRequired();

            builder.Property(x => x.ContactNumber)
                .IsRequired();

            builder.HasIndex(x => x.CreatedById);
        }
    }
}
