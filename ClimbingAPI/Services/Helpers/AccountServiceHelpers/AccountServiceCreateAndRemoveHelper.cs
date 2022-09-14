using ClimbingAPI.Entities;
using ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers
{
    public class AccountServiceCreateAndRemoveHelper: IAccountServiceCreateAndRemoveHelper
    {
        private IAccountServiceGetDataHelper _accountServiceGetDataHelper;
        private readonly ClimbingDbContext _dbContext;

        public AccountServiceCreateAndRemoveHelper(IAccountServiceGetDataHelper accountServiceGetDataHelper, ClimbingDbContext dbContext)
        {
            _accountServiceGetDataHelper = accountServiceGetDataHelper;
            _dbContext = dbContext;
        }
        public async Task RemoveUserAndUserClimbingSpotEntites(User user, int userId)
        {
            var userClimbingSpotEntities = await _accountServiceGetDataHelper.GetUserClimbingSpotsEntities(userId);
            _dbContext.UserClimbingSpotLinks.RemoveRange(userClimbingSpotEntities);

            _dbContext.User.Remove(user);
            _dbContext.SaveChanges();
        }

        public UserClimbingSpotLinks CreateUserClimbingSpotEntity(int userId, int roleId)
        {
            return new UserClimbingSpotLinks()
            {
                UserId = userId,
                ClimbingSpotId = null,
                RoleId = roleId
            };
        }

    }
}
