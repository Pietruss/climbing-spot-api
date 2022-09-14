using ClimbingAPI.Entities;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers
{
    public class AccountServiceGetDataHelper: IAccountServiceGetDataHelper
    {
        private readonly ClimbingDbContext _dbContext;
        public AccountServiceGetDataHelper(ClimbingDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<UserClimbingSpotLinks>> GetUserClimbingSpotsEntities(int userId)
        {
            return await _dbContext
                .UserClimbingSpotLinks
                .Where(x => x.UserId == userId)
                .Select(x => new UserClimbingSpotLinks() { Id = x.Id })
                .ToListAsync();
        }

        public async Task<User> GetUserById(int userId)
        {
            return await _dbContext
                 .User
                 .Where(x => x.Id == userId)
                 .Select(x => new User() { Id = x.Id, PasswordHash = x.PasswordHash, Email = x.Email, FirstName = x.FirstName, LastName = x.LastName, DateOfBirth = x.DateOfBirth, CreatedByUserId = x.CreatedByUserId, CreationDateTime = x.CreationDateTime })
                 .FirstOrDefaultAsync();
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbContext
                .User
                .Where(x => x.Email == email)
                .Select(x => new User() { Id = x.Id, PasswordHash = x.PasswordHash, FirstName = x.FirstName, LastName = x.LastName, DateOfBirth = x.DateOfBirth })
                .FirstOrDefaultAsync();
        }
    }
}
