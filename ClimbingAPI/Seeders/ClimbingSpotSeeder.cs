using ClimbingAPI.Entities;
using ClimbingAPI.Entities.Address;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ClimbingAPI.Seeders
{
    public class ClimbingSpotSeeder
    {
        private readonly ClimbingDbContext _dbContext;

        public ClimbingSpotSeeder(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Database.IsRelational())
                {
                    if (!_dbContext.ClimbingSpot.Any())
                    {
                        // _dbContext.ClimbingSpot.AddRange(GetClimbingSpot());
                        // _dbContext.SaveChanges();
                    }
                }
            }
        }

        private IEnumerable<ClimbingSpot> GetClimbingSpot()
        {
            var climbingSpots = new List<ClimbingSpot>()
            {
                new ClimbingSpot()
                {
                    Id = 1,
                    Address = new Address()
                    {
                        Id = 1,
                        Author = "admin",
                        City = "Krakow",
                        Street = "Wizjonerow"
                    },
                    Name = "Forteca",
                    Description = "test test test",
                    CreatedBy = new User()
                    {
                        Id = 1,
                        Email = "test@gmail.com",
                        FirstName = "Piotr",
                        LastName = "Piotrowski",
                        PasswordHash = "test"
                    },
                    ContactEmail = "contact@gmail.com",
                    ContactNumber = "123321123",
                    AddressId = 1,
                    Author = "admin"
                }
            };
            
            return climbingSpots;
        }
    }
}
