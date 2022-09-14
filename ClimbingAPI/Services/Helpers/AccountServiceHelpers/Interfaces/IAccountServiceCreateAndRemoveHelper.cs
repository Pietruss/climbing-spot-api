using ClimbingAPI.Entities;
using System.Threading.Tasks;

namespace ClimbingAPI.Services.Helpers.AccountServiceHelpers.Interfaces
{
    public interface IAccountServiceCreateAndRemoveHelper
    {
        Task RemoveUserAndUserClimbingSpotEntites(User user, int userId);
        UserClimbingSpotLinks CreateUserClimbingSpotEntity(int userId, int roleId);
    }
}
