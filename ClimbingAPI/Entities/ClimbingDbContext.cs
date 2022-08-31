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
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
