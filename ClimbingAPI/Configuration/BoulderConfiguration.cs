using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Boulder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClimbingAPI.Configuration
{
    public class BoulderConfiguration : IEntityTypeConfiguration<Boulder>
    {
        public void Configure(EntityTypeBuilder<Boulder> builder)
        {
            builder.Property(x => x.Name)
                .IsRequired();
            
            builder.HasOne(x => x.Image)
                .WithOne(x => x.Boulder)
                .HasForeignKey<Image>(x => x.BoulderId);
        }
    }
}
