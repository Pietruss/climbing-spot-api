using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClimbingAPI.Entities;

namespace ClimbingAPI
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
                if (!_dbContext.Role.Any())
                {
                    _dbContext.Role.AddRange(GetRoles());
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>()
            {
                new Role()
                {
                    Name = "Admin"
                },
                new Role()
                {
                    Name = "Manager"
                },
                new Role()
                {
                    Name = "Bald Builder"
                },
                new Role()
                {
                    Name = "User"
                }
            };

            return roles;
        }
    }
}
