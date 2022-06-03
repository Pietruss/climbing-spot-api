using ClimbingAPI.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace ClimbingAPI.Seeders
{
    public class UserClimbingSpotSeeder
    {
        private readonly ClimbingDbContext _dbContext;

        public UserClimbingSpotSeeder(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect())
            {
                if (!_dbContext.Database.IsRelational())
                {
                    if (!_dbContext.UserClimbingSpot.Any())
                    {
                        // _dbContext.UserClimbingSpot.AddRange(GetUserClimbingSpot());
                        // _dbContext.SaveChanges();
                    }
                }
            }
        }

        private IEnumerable<UserClimbingSpot> GetUserClimbingSpot()
        {
            var userClimbingSpots = new List<UserClimbingSpot>()
            {
                new UserClimbingSpot()
                {
                    ClimbingSpotId = null,
                    RoleId = 1,
                    UserId = 1,
                    Id = 1
                },
                new UserClimbingSpot()
                {
                    ClimbingSpotId = null,
                    RoleId = 3,
                    UserId = 2,
                    Id = 2
                }
            };

            return userClimbingSpots;
        }
    }
}
