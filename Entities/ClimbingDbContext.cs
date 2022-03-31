using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClimbingAPI.Entities
{
    public class ClimbingDbContext: DbContext
    {
        private string _connectionString = "Server=localhost;Database=ClimbingSpot;Trusted_Connection=True";
        public DbSet<ClimbingSpot> ClimbingSpot { get; set; }
        public DbSet<Address.Address> Addresses { get; set; }
        public DbSet<Boulder.Boulder> Boulder { get; set; }

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
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
