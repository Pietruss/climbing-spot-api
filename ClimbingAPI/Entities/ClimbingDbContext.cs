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
        public DbSet<UserClimbingSpot> UserClimbingSpot { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ClimbingSpot>()
                .Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<ClimbingSpot>()
                .Property(x => x.ContactEmail)
                .IsRequired();

            modelBuilder.Entity<ClimbingSpot>()
                .Property(x => x.ContactNumber)
                .IsRequired();


            modelBuilder.Entity<Address.Address>()
                .Property(x => x.City)
                .IsRequired();

            modelBuilder.Entity<Boulder.Boulder>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .Property(x => x.Name)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.Email)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.LastName)
                .IsRequired();

            modelBuilder.Entity<User>()
                .Property(x => x.FirstName)
                .IsRequired();

        }
    }
}
