using Microsoft.EntityFrameworkCore;

namespace ClimbingAPI.Entities
{
    public class ClimbingDbContext: DbContext
    {
        public ClimbingDbContext(DbContextOptions<ClimbingDbContext> options) : base(options) 
        {}
        public DbSet<ClimbingSpot> ClimbingSpot { get; set; }
        public DbSet<Address.Address> Addresses { get; set; }
        public DbSet<Boulder.Boulder> Boulder { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<UserClimbingSpotLinks> UserClimbingSpotLinks { get; set; }
        public DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClimbingSpot>(eb =>
            {
                eb.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(150);

                eb.Property(x => x.ContactEmail)
                    .IsRequired();

                eb.Property(x => x.ContactNumber)
                    .IsRequired();
            });

            modelBuilder.Entity<Address.Address>()
                .Property(x => x.City)
                .IsRequired();

            modelBuilder.Entity<Boulder.Boulder>(eb =>
            {
                eb.Property(x => x.Name)
                .IsRequired();

                eb.HasOne(x => x.Image)
                .WithOne(x => x.Boulder)
                .HasForeignKey<Image>(x => x.BoulderId);
            });
            
            modelBuilder.Entity<Role>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<User>(eb =>
            {
                eb.Property(x => x.Email)
                    .IsRequired();

                eb.Property(x => x.LastName)
                    .IsRequired();

                eb.Property(x => x.FirstName)
                    .IsRequired();
            });
        }
    }
}
